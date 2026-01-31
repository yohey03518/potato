using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Potato.Client.External.Fugle;

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
                var symbolId = "2330";
                logger.LogInformation("Fetching stock price for {SymbolId}...", symbolId);
                
                var result = await fugleApiClient.GetIntradayQuoteAsync(symbolId);
                
                logger.LogInformation("Successfully fetched stock price for {SymbolId}.", symbolId);
                logger.LogInformation("Result: {Result}", result);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching the stock price.");
        }

        logger.LogInformation("StockPriceMonitorService has completed its task. Shutting down application...");
        hostApplicationLifetime.StopApplication();
    }
}
