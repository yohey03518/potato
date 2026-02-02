using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Potato.Core.Interfaces;
using Potato.Core.Entities;

namespace Potato.Client.Services;

public class StockPriceMonitorService(
    IMarketDataProxy marketDataProxy,
    IEnumerable<IStrategy> strategies,
    ILogger<StockPriceMonitorService> logger)
    : BackgroundService
{
    private readonly HashSet<string> _monitoringPool = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("StockPriceMonitorService is starting.");

        // 1. Fetch Full Snapshot for initial filtering
        var tseData = await marketDataProxy.GetSnapshotQuotesAsync("TSE");
        var otcData = await marketDataProxy.GetSnapshotQuotesAsync("OTC");
        var allStocks = tseData.Concat(otcData).ToList();

        // 2. Filter Data (Volume > 5000)
        const long volumeThreshold = 5000;
        var volumeCandidates = allStocks
            .Where(s => s.TradeVolume > volumeThreshold)
            .ToList();

        // 3. Filter Data (Yesterday's Price > 20MA)
        var candidates = new List<StockSnapshot>();
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Fetch enough history to ensure we get T-1 data (e.g., last 10 days to cover weekends/holidays)
        var historyFromDate = DateTime.Now.AddDays(-20).ToString("yyyy-MM-dd");

        logger.LogInformation("Applying 20MA Filter on {Count} candidates...", volumeCandidates.Count);

        // Process in batches or one by one. For simplicity here, sequential.
        foreach (var stock in volumeCandidates)
        {
            if (stoppingToken.IsCancellationRequested) break;

            try 
            {
                // Fetch 20MA
                var smaData = await marketDataProxy.GetTechnicalSmaAsync(stock.Symbol, 20, historyFromDate, currentDate);
                
                if (smaData == null || !smaData.Any())
                {
                    logger.LogWarning("No SMA data found for {Symbol}. Skipping.", stock.Symbol);
                    continue;
                }

                // Get the latest available SMA date (T-1 or T if market closed today). 
                // Since Snapshot is "Real-time" or "Last Close", we need "Yesterday's" price vs "Yesterday's" 20MA?
                // The requirement is "price of yesterday was greater than 20MA".
                // If today is trading day T, "Yesterday" is T-1.
                // Snapshot data gives us current price (or T close if market closed).
                // Let's assume we want to compare T-1 Close > T-1 20MA.
                
                // Sort by date descending
                var orderedSma = smaData.OrderByDescending(d => d.Date).ToList();
                // Taking the most recent completed day's SMA.
                var lastSma = orderedSma.FirstOrDefault();
                
                if (lastSma == null) continue;

                // Derive "Yesterday's" Close Price from Snapshot?
                // Snapshot gives: ClosePrice (Current/Last Traded), Change.
                // If data is real-time (T), then ClosePrice is T. PrevClose = ClosePrice - Change.
                // If data is after-market (T), ClosePrice is T.
                // The request says: "price of yesterday".
                
                // Let's look at the Snapshot Date.
                // But Snapshot object doesn't have the date field exposed effectively in the list (as per our previous finding).
                // However, `Change` is universally "Current Price - Previous Close".
                // So `Previous Close` = `ClosePrice` - `Change`.
                // If the market is open (T), `Previous Close` is definitely T-1 Close.
                // If the market is closed (T), `ClosePrice` is T Close. `Previous Close` is T-1 Close.
                
                // CAUTION: Fugle Snapshot `ClosePrice` is the *latest* trade price.
                // If market is open, `ClosePrice` is current price.
                // `Previous Close` (Yesterday's Close) = `ClosePrice` - `Change`.
                
                if (stock.ClosePrice == null) continue;
                
                decimal comparisonPrice;
                DateOnly targetSmaDate;
                var today = DateOnly.FromDateTime(DateTime.Now);

                // Analyze Snapshot Date to determine "Yesterday" relative to the data
                if (stock.Date < today)
                {
                    // Case 1: Snapshot is from a previous day (e.g. Market closed/Pre-market).
                    // The "ClosePrice" represents the Final Close of that day (T-1).
                    // The user wants "Price of Yesterday".
                    // If we are at T (Today), "Yesterday" is T-1.
                    // The Snapshot *IS* T-1.
                    // So we use Snapshot Close vs SMA(T-1).
                    
                    comparisonPrice = stock.ClosePrice.Value;
                    targetSmaDate = stock.Date;
                }
                else
                {
                    // Case 2: Snapshot is from Today (T).
                    // This means market is Open or data has rolled over.
                    // "ClosePrice" is Current Price (T).
                    // User wants "Price of Yesterday" (T-1).
                    // We derive T-1 Close from T Current - T Change.
                    
                    if (stock.Change == null) continue;
                    comparisonPrice = stock.ClosePrice.Value - stock.Change.Value;
                    
                    // We need SMA(T-1).
                    // Since stock.Date is T, we need the latest SMA *before* T.
                    // We can't just subtract 1 day because of weekends. 
                    // We will look for the first SMA date < stock.Date.
                    // (Assuming requested history covers it).
                    
                     // We don't have a simple date variable for "T-1" without looking at data/calendar.
                     // But we can search the SMA list for first date < stock.Date.
                     targetSmaDate = stock.Date; // Placeholder, logic below handles finding < Date
                }

                // Find the SMA matching the target requirement
                SmaData? targetSma = null;

                if (stock.Date < today)
                {
                     // We want SMA on the exact same day as the Snapshot (T-1)
                     targetSma = smaData.FirstOrDefault(d => d.Date == stock.Date);
                }
                else
                {
                     // We want SMA for the day *before* the Snapshot (T-1)
                     targetSma = orderedSma.FirstOrDefault(d => d.Date < stock.Date);
                }
                
                if (targetSma == null) 
                {
                    // If strict match fails, log?
                    continue;
                }
                
                if (comparisonPrice > targetSma.Value)
                {
                    candidates.Add(stock);
                }
                
                // Small delay to be nice to API
                await Task.Delay(50, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to filter {Symbol} for 20MA", stock.Symbol);
            }
        }

        logger.LogInformation("Found {Count} candidates for strategy scanning.", candidates.Count);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Starting strategy scan cycle...");

                foreach (var stock in candidates.TakeWhile(stock => !stoppingToken.IsCancellationRequested).Where(stock => !_monitoringPool.Contains(stock.Symbol)))
                {
                    // 3. Fetch Real-time Quote for Strategy Evaluation
                    var quote = await marketDataProxy.GetIntradayQuoteAsync(stock.Symbol);
                    if (quote == null) continue;

                    // 4. Evaluate all enabled strategies
                    foreach (var strategy in strategies)
                    {
                        var signal = strategy.Evaluate(quote);
                        if (signal == null) continue;
                        logger.LogInformation("[SIGNAL] {Strategy} triggered for {Symbol} at {Price}. Action: {Action}", 
                            strategy.Name, signal.Symbol, signal.Price, signal.Action);
                            
                        // Simulate placing order and entering monitoring pool
                        _monitoringPool.Add(signal.Symbol);
                        logger.LogInformation("Stock {Symbol} added to Monitoring Pool. (Total: {Count})", signal.Symbol, _monitoringPool.Count);
                            
                        // Since we only want one strategy to pick a stock at a time (First-Match)
                        break;
                    }
                    
                    // Small delay to avoid hitting API rate limits during scan
                    await Task.Delay(100, stoppingToken);
                }

                logger.LogInformation("Scan cycle completed. Waiting for next cycle...");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Wait before next full scan
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during strategy scan.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
