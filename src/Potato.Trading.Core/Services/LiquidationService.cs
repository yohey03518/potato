using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;

namespace Potato.Trading.Core.Services;

public class LiquidationService
{
    private readonly ILogger<LiquidationService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly TradingService _tradingService;
    private readonly MarketClock _clock;
    
    // Config: 13:25
    private readonly TimeSpan _liquidationTime = new TimeSpan(13, 25, 0);

    public LiquidationService(
        ILogger<LiquidationService> logger, 
        IOrderRepository orderRepository,
        TradingService tradingService,
        MarketClock clock)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _tradingService = tradingService;
        _clock = clock;
        
        _clock.OnMinuteTick += CheckLiquidationTime;
    }

    public void CheckLiquidationTime(DateTime currentTime)
    {
        // Convert to Taipei Time or assume currentTime is aligned.
        // Assuming currentTime is UTC, Taipei is UTC+8.
        var localTime = currentTime.AddHours(8); 
        
        if (localTime.TimeOfDay >= _liquidationTime && localTime.TimeOfDay < _liquidationTime.Add(TimeSpan.FromMinutes(1)))
        {
            _logger.LogWarning("Liquidation Time Reached! Triggering Forced Liquidation.");
            // Fire and forget or async void (be careful)
            _ = PerformLiquidationAsync();
        }
    }

    public async Task PerformLiquidationAsync()
    {
        // 1. Cancel all pending orders
        // _tradingService.CancelAllOrders(); // Need to implement this in TradingService
        
        // 2. Flatten all positions
        // _tradingService.CloseAllPositions(); // Need to implement this in TradingService
        
        // For T022, I will simulate calling logic on TradingService or implement placeholder here.
        // Since TradingService holds the state _activeOrders and _positions (private),
        // I should add methods to TradingService to support this.
        
        _logger.LogInformation("Liquidation Logic Executing...");
        
        // Logic:
        // Get all active orders from repo or service -> Cancel them.
        // Get all open positions -> Send Market Orders to close.
    }
}
