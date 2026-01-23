using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Potato.Trading.Core.Services;

public class MarketClock
{
    private readonly ILogger<MarketClock> _logger;

    public event Action<DateTime>? OnMinuteTick;

    public MarketClock(ILogger<MarketClock> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow; // Or localized time
            
            // Assuming simulated time or real-time. 
            // If real-time, we invoke every minute.
            
            OnMinuteTick?.Invoke(now);

            try
            {
                // Align to next minute start
                var delay = TimeSpan.FromSeconds(60 - now.Second);
                await Task.Delay(delay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
