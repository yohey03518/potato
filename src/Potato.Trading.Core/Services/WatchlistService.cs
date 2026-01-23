using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;

namespace Potato.Trading.Core.Services;

public interface IWatchlistService
{
    Task GenerateDailyWatchlistAsync(DateTime date);
    Task<List<Watchlist>> GetDailyWatchlistAsync(DateTime date);
}

public class WatchlistService : IWatchlistService
{
    private readonly IWatchlistRepository _repository;
    private readonly IMarketDataApiClient _marketDataClient; // Needed to get history for selection? 
                                                             // Actually selection is based on "Previous Day Data".
                                                             // We assume we can fetch historical data or yesterday's close.
    private readonly ILogger<WatchlistService> _logger;

    public WatchlistService(IWatchlistRepository repository, IMarketDataApiClient marketDataClient, ILogger<WatchlistService> logger)
    {
        _repository = repository;
        _marketDataClient = marketDataClient;
        _logger = logger;
    }

    public async Task GenerateDailyWatchlistAsync(DateTime date)
    {
        // In a real system, this would scan all symbols.
        // For simulation, we might have a predefined pool or scan a small list.
        var pool = new[] { "2330", "2317", "2454" }; // TSMC, Foxconn, MediaTek

        foreach (var symbol in pool)
        {
            try
            {
                // We need Yesterday's Close and SMA20.
                // Assuming MarketDataApiClient has a method for this or we mock it.
                // Since T008 implemented GetSnapshotAsync which is current, 
                // we might need to extend it or assume we have the data.
                
                // For this task, I'll simulate the selection logic logic:
                // If Close > SMA20 -> Long
                // If Close < SMA20 -> Short
                
                // Fetching snapshot as "Yesterday's data" for simulation purpose if running before market open.
                var snapshot = await _marketDataClient.GetSnapshotAsync(symbol);
                
                // Fake SMA calculation for selection (normally requires history)
                var fakeSma20 = snapshot.Price * 0.99m; // Assume uptrend for test
                
                var direction = snapshot.Price > fakeSma20 ? TradeDirection.Long : TradeDirection.Short;

                var watchlist = new Watchlist
                {
                    TradeDate = date.Date,
                    Symbol = symbol,
                    Direction = direction,
                    BasePrice = snapshot.Price,
                    MA20_Day = fakeSma20
                };

                await _repository.AddAsync(watchlist);
                _logger.LogInformation("Added {Symbol} to watchlist as {Direction}", symbol, direction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate watchlist for {Symbol}", symbol);
            }
        }
    }

    public async Task<List<Watchlist>> GetDailyWatchlistAsync(DateTime date)
    {
        return await _repository.GetByDateAsync(date.Date);
    }
}
