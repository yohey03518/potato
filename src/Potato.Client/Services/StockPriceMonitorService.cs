using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Potato.Client.External.Fugle;

namespace Potato.Client.Services;

public class StockPriceMonitorService : BackgroundService
{
    private readonly IFugleApiClient _fugleApiClient;
    private readonly ILogger<StockPriceMonitorService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public StockPriceMonitorService(
        IFugleApiClient fugleApiClient, 
        ILogger<StockPriceMonitorService> logger,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _fugleApiClient = fugleApiClient;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StockPriceMonitorService is starting.");

        try
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                var symbolId = "2330";
                _logger.LogInformation("Fetching stock price for {SymbolId}...", symbolId);
                
                var result = await _fugleApiClient.GetIntradayQuoteAsync(symbolId);
                
                _logger.LogInformation("Successfully fetched stock price for {SymbolId}.", symbolId);
                _logger.LogInformation("Result: {Result}", result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the stock price.");
        }

        _logger.LogInformation("StockPriceMonitorService has completed its task. Shutting down application...");
        _hostApplicationLifetime.StopApplication();
    }
}
