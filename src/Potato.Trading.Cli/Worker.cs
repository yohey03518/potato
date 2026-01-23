using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Potato.Trading.Cli.UI;
using Potato.Trading.Core.Interfaces;
using Potato.Trading.Core.Services;

namespace Potato.Trading.Cli;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dashboard _dashboard;

    public Worker(
        ILogger<Worker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _dashboard = new Dashboard();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

        using (var scope = _serviceProvider.CreateScope())
        {
            var clock = scope.ServiceProvider.GetRequiredService<MarketClock>();
            var marketDataService = scope.ServiceProvider.GetRequiredService<IMarketDataService>();
            var tradingService = scope.ServiceProvider.GetRequiredService<TradingService>();
            var liquidationService = scope.ServiceProvider.GetRequiredService<LiquidationService>();

            // Start Clock
            var clockTask = clock.StartAsync(stoppingToken);

            // Start Market Data (Placeholder)
            // await marketDataService.StartAsync(stoppingToken);

            // Run UI
            // await _dashboard.RunAsync(stoppingToken);
            
            // Wait for cancellation
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Graceful shutdown
            }
        }
    }
}
