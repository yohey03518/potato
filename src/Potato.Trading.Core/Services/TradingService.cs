using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Potato.Trading.Core.Domain;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;

namespace Potato.Trading.Core.Services;

public class TradingOptions
{
    public decimal TotalCapital { get; set; } = 200000; // Default per spec T011 FR-011 says 200k per trade, but T003 default config says 1M total.
                                                        // FR-011: "Every trade fixed 200,000"
}

public class TradingService
{
    private readonly ILogger<TradingService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IExecutionRepository _executionRepository;
    private readonly StrategyEvaluator _evaluator;
    private readonly MatchingEngine _matchingEngine;
    private readonly OrderBookTracker _tracker;
    private readonly TradingOptions _options;

    // In-memory state
    private readonly ConcurrentDictionary<string, Order> _activeOrders = new();
    private readonly ConcurrentDictionary<string, int> _positions = new(); // Symbol -> Signed Quantity (+Long, -Short)

    public TradingService(
        ILogger<TradingService> logger, 
        IOrderRepository orderRepository,
        IExecutionRepository executionRepository,
        MatchingEngine matchingEngine,
        IOptions<TradingOptions> options)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _executionRepository = executionRepository;
        _matchingEngine = matchingEngine; // Ideally registered as Singleton or Scoped
        _tracker = new OrderBookTracker();
        _evaluator = new StrategyEvaluator();
        _options = options.Value;
    }

    public async Task OnKLineClosedAsync(KLine kline)
    {
        // 1. Evaluate Strategy
        var currentPosition = _positions.ContainsKey(kline.Symbol) ? _positions[kline.Symbol] : 0;
        
        // Determine allowed direction based on position or Watchlist (omitted here, assumed passed or looked up)
        // For simplicity, if Pos > 0 we look to Sell, if Pos < 0 we look to Cover, if Pos == 0 we look to Enter based on Watchlist.
        
        // Assuming we fetched Watchlist direction earlier.
        var direction = TradeDirection.Long; // Placeholder

        var signal = _evaluator.Evaluate(kline, direction);

        if (signal.Type != SignalType.None)
        {
            _logger.LogInformation("Signal Generated: {Type} for {Symbol} at {Price}", signal.Type, signal.Symbol, signal.TriggerPrice);
            await ExecuteSignalAsync(signal);
        }
    }

    public async Task OnMarketDataAsync(MarketData marketData)
    {
        // 1. Check Active Orders for Matching
        if (_activeOrders.TryGetValue(marketData.Symbol, out var order))
        {
            _matchingEngine.ProcessMarketData(order, marketData);
            
            if (order.Status == OrderStatus.Filled)
            {
                await HandleFillAsync(order);
            }
            else
            {
                // Update Order if changed?
            }
        }
    }

    private async Task ExecuteSignalAsync(StrategySignal signal)
    {
        // Check funds, risk check (FR-011)
        // Fixed 200k capital per trade
        decimal budget = 200000;
        decimal price = signal.TriggerPrice;
        
        if (price > 100) 
        {
            _logger.LogInformation("Price > 100 ({Price}), ignoring signal.", price);
            return;
        }

        int quantity = (int)(budget / price);
        quantity = (quantity / 1000) * 1000; // Round down to lots (1000 shares)

        if (quantity < 2000)
        {
            _logger.LogInformation("Quantity < 2000 ({Qty}), ignoring signal.", quantity);
            return;
        }

        // Create Order
        var order = new Order
        {
            Symbol = signal.Symbol,
            Price = price, // Assuming Limit Order at Signal Price
            Quantity = quantity,
            Type = OrderType.Limit,
            Status = OrderStatus.Pending,
            Side = signal.Type == SignalType.Buy || signal.Type == SignalType.Cover ? OrderSide.Buy : OrderSide.Sell,
            QueuePositionVolume = 0 // Needs to be initialized with current MarketData Snapshot!
            // Note: We need current MarketData to init queue position correctly.
            // If OnKLineClosed doesn't have it, we might assume 0 or fetch it.
        };

        // Persist
        await _orderRepository.AddAsync(order);
        _activeOrders[order.Symbol] = order;
        
        _logger.LogInformation("Order Placed: {Side} {Qty} {Symbol} @ {Price}", order.Side, order.Quantity, order.Symbol, order.Price);
    }

    private async Task HandleFillAsync(Order order)
    {
        _logger.LogInformation("Order Filled: {Id}", order.Id);
        
        // Update Position
        int qty = order.Side == OrderSide.Buy ? order.Quantity : -order.Quantity;
        _positions.AddOrUpdate(order.Symbol, qty, (_, current) => current + qty);

        // Update DB
        await _orderRepository.UpdateAsync(order);
        
        // Remove from active tracking
        _activeOrders.TryRemove(order.Symbol, out _);
        
        // Create Execution Entity
        var execution = new Execution
        {
            OrderId = order.Id,
            Price = order.Price,
            Quantity = order.FilledVolume,
            ExecutedAt = DateTime.UtcNow,
            // Calculate Fee and Tax (Simplified)
            Fee = order.Price * order.FilledVolume * 0.001425m,
            Tax = order.Side == OrderSide.Sell ? order.Price * order.FilledVolume * 0.003m : 0
        };
        
        await _executionRepository.AddAsync(execution);
    }
}
