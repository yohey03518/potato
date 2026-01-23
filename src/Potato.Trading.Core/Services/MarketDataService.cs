using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;

namespace Potato.Trading.Core.Services;

public class MarketDataService : IMarketDataService
{
    private readonly IMarketDataStreamClient _streamClient;
    private readonly ILogger<MarketDataService> _logger;
    private readonly ConcurrentDictionary<string, KLine> _currentKLines = new();

    public event Action<KLine>? OnKLineClosed;

    public MarketDataService(IMarketDataStreamClient streamClient, ILogger<MarketDataService> logger)
    {
        _streamClient = streamClient;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // For demonstration, we might start watching a default list or this method would accept symbols.
        // The spec implies WatchlistService determines what to watch. 
        // We'll assume WatchlistService calls this or we expose a Subscribe method.
        // For T012, we focus on the aggregation logic.
    }

    public async Task SubscribeAsync(string symbol, CancellationToken cancellationToken)
    {
        await _streamClient.ConnectAsync(symbol, async (marketData) =>
        {
            ProcessTick(marketData);
            await Task.CompletedTask;
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _streamClient.DisconnectAsync();
    }

    private void ProcessTick(MarketData marketData)
    {
        var timestamp = marketData.Timestamp;
        // 5-minute bucket: 09:00, 09:05, ...
        // Floor to nearest 5 min
        var bucketMinutes = (timestamp.Minute / 5) * 5;
        var bucketStart = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, bucketMinutes, 0, timestamp.Kind);

        _currentKLines.AddOrUpdate(marketData.Symbol, 
            // Add new KLine
            (symbol) => new KLine
            {
                Symbol = symbol,
                StartTime = bucketStart,
                Open = marketData.Price,
                High = marketData.Price,
                Low = marketData.Price,
                Close = marketData.Price,
                Volume = marketData.Volume
            },
            // Update existing KLine
            (symbol, current) =>
            {
                if (current.StartTime != bucketStart)
                {
                    // Previous candle closed
                    OnKLineClosed?.Invoke(current);
                    
                    // Start new candle
                    return new KLine
                    {
                        Symbol = symbol,
                        StartTime = bucketStart,
                        Open = marketData.Price,
                        High = marketData.Price,
                        Low = marketData.Price,
                        Close = marketData.Price,
                        Volume = marketData.Volume
                    };
                }

                // Update current candle
                current.High = Math.Max(current.High, marketData.Price);
                current.Low = Math.Min(current.Low, marketData.Price);
                current.Close = marketData.Price;
                current.Volume += marketData.Volume; // Assuming Tick volume is incremental for that tick, not cumulative total
                
                return current;
            });
    }
}
