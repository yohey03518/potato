using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Potato.Client.External.Fugle;
using Potato.Client.External.Fugle.Models;

namespace Potato.Client.Services;

public class StockPriceMonitorService(
    IFugleApiClient fugleApiClient,
    ILogger<StockPriceMonitorService> logger,
    IHostApplicationLifetime hostApplicationLifetime)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("StockPriceMonitorService is starting.");

        try
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Fetching snapshot quotes for TSE and OTC...");

                var tseData = new List<SnapshotData>();
                var otcData = new List<SnapshotData>();
                bool permissionDenied = false;

                try 
                {
                    // 1. Try Fetch Full Snapshot
                    var tseTask = fugleApiClient.GetSnapshotQuotesAsync("TSE");
                    var otcTask = fugleApiClient.GetSnapshotQuotesAsync("OTC");
                    await Task.WhenAll(tseTask, otcTask);
                    tseData = await tseTask;
                    otcData = await otcTask;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    logger.LogWarning("Snapshot Quotes API is forbidden (403). API Key does not have sufficient permissions.");
                    permissionDenied = true;
                }

                if (permissionDenied)
                {
                    Console.WriteLine("\n[API Permission Error] Unable to fetch stock list for filtering.");
                    Console.WriteLine("The 'Snapshot Quotes' API requires a Developer or Advanced plan.");
                    Console.WriteLine("Please upgrade your Fugle API key to use this feature.\n");
                }
                else
                {
                    var allStocks = tseData.Concat(otcData).ToList();
                    logger.LogInformation("Total stocks fetched: {Count}", allStocks.Count);

                    // 2. Filter Data
                    // Rule 1: Not ETF (API 'COMMONSTOCK' handles this)
                    // Rule 2: Yesterday Volume > 5000 (Sheets/Lots)
                    long volumeThreshold = 5000;

                    var filteredStocks = allStocks
                        .Where(s => s.TradeVolume > volumeThreshold)
                        .ToList();

                    logger.LogInformation("Found {Count} stocks with volume > {Threshold} sheets.", filteredStocks.Count, volumeThreshold);
                    
                    // 3. Output Results
                    Console.WriteLine($"\n=== Matched Stocks (Total: {filteredStocks.Count}) ===");
                    foreach (var stock in filteredStocks)
                    {
                        Console.WriteLine($"{stock.Symbol} ({stock.Name}): Volume {stock.TradeVolume}");
                    }
                    Console.WriteLine("==============================================\n");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching or filtering stock data.");
        }

        logger.LogInformation("StockPriceMonitorService has completed its task. Shutting down application...");
        hostApplicationLifetime.StopApplication();
    }
}
