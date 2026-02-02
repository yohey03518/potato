using Microsoft.Extensions.Logging;
using Potato.Core.Entities;
using Potato.Core.Interfaces;

namespace Potato.Client.Services;

public class MarketScanCandidateProvider(
    IMarketDataProxy marketDataProxy,
    ILogger<MarketScanCandidateProvider> logger)
    : IInitialCandidateProvider
{
    private static readonly SemaphoreSlim _semaphore = new(5); // Throttle concurrent API calls

    public async Task<List<StockSnapshot>> GetAsync()
    {
        logger.LogInformation("Starting initial candidate filtering...");

        // 1. Fetch All Snapshots
        var tseData = await marketDataProxy.GetSnapshotQuotesAsync("TSE");
        var otcData = await marketDataProxy.GetSnapshotQuotesAsync("OTC");
        var allStocks = tseData.Concat(otcData).ToList();

        logger.LogInformation("Total stocks fetched: {Count}. Checking history for valid candidates...", allStocks.Count);
        
        var candidates = new List<StockSnapshot>();
        var tasks = allStocks.Select(async stock =>
        {
            await _semaphore.WaitAsync();
            try
            {
                if (await CheckCandidateAsync(stock))
                {
                    lock (candidates)
                    {
                        candidates.Add(stock);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        logger.LogInformation("Filtering complete. Found {Count} candidates.", candidates.Count);
        return candidates;
    }

    private async Task<bool> CheckCandidateAsync(StockSnapshot stock)
    {
        try
        {
            // Fetch Candles (History)
            // We need enough days to calculate 20MA. 
            // 20 trading days is ~4 weeks. Fetch 50 calendar days to be safe.
            var to = DateTime.Today.ToString("yyyy-MM-dd");
            var from = DateTime.Today.AddDays(-50).ToString("yyyy-MM-dd");

            var candles = await marketDataProxy.GetTechnicalCandlesAsync(stock.Symbol, from, to);
            
            // Need at least 21 candles (20 for MA + 1 target)
            if (candles.Count < 21) return false;

            // Sort by Date Ascending
            var sortedCandles = candles.OrderBy(c => c.Date).ToList();

            // Find "Yesterday's" data (The last completed candle before Today)
            var today = DateOnly.FromDateTime(DateTime.Today);
            var targetCandle = sortedCandles.LastOrDefault(c => c.Date < today);

            if (targetCandle == null) return false;

            // 1. Check Volume > 5000 (Yesterday's Volume)
            if (targetCandle.Volume <= 5000) return false;

            // 2. Calculate 20MA for TargetCandle
            // We need 20 candles ending at TargetCandle (inclusive)
            var targetIndex = sortedCandles.IndexOf(targetCandle);
            if (targetIndex < 19) return false;

            var sma20Segment = sortedCandles.Skip(targetIndex - 19).Take(20).Select(c => c.Close);
            var sma20 = sma20Segment.Average();

            // 3. Price > 20MA
            if (targetCandle.Close > sma20)
            {
                 logger.LogDebug("Candidate Found: {Symbol} [{Date}], Vol: {Vol}, Close: {Close}, SMA20: {SMA}", stock.Symbol, targetCandle.Date, targetCandle.Volume, targetCandle.Close, sma20);
                 return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            // Log full error only for debugging if needed, concise log for normal ops
            logger.LogTrace(ex, "Error checking candidate {Symbol}", stock.Symbol);
            return false;
        }
    }
}
