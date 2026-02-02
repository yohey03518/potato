using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Potato.Core.Interfaces;
using Potato.Core.Entities;

namespace Potato.Client.Services;

public class StockPriceMonitorService(
    IMarketDataProxy marketDataProxy,
    IInitialCandidateFilter initialCandidateFilter,
    IEnumerable<IStrategy> strategies,
    ILogger<StockPriceMonitorService> logger)
    : BackgroundService
{
    private readonly HashSet<string> _monitoringPool = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("StockPriceMonitorService is starting.");

        var candidates = await initialCandidateFilter.FilterAsync(stoppingToken);

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
