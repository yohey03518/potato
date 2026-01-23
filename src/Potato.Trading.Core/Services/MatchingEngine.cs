using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Potato.Trading.Core.Domain;
using Potato.Trading.Core.Entities;

namespace Potato.Trading.Core.Services;

public class MatchingEngine
{
    private readonly ILogger<MatchingEngine> _logger;
    private readonly OrderBookTracker _tracker;

    public MatchingEngine(ILogger<MatchingEngine> logger)
    {
        _logger = logger;
        _tracker = new OrderBookTracker();
    }

    public void ProcessMarketData(Order order, MarketData marketData)
    {
        if (order.Status == OrderStatus.Filled || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Rejected)
            return;

        if (order.Type == OrderType.Market)
        {
            // Market Order: Match against best opposite side immediately
            ExecuteMarketOrder(order, marketData);
        }
        else if (order.Type == OrderType.Limit)
        {
            ExecuteLimitOrder(order, marketData);
        }
    }

    private void ExecuteMarketOrder(Order order, MarketData marketData)
    {
        // Simple simulation: Fill at current Price (Last Trade) or Best Bid/Ask?
        // Spec says: "系統應以當下最佳對手價模擬成交"
        
        if (order.Side == OrderSide.Buy)
        {
            // Buy at Ask
            // If we have Ask prices
            if (marketData.AskPrices.Length > 0)
            {
                FillOrder(order, marketData.AskPrices[0], order.Quantity, marketData.Timestamp);
            }
            else
            {
                // Fallback to latest price if no Ask available (or wait)
                FillOrder(order, marketData.Price, order.Quantity, marketData.Timestamp);
            }
        }
        else
        {
            // Sell at Bid
            if (marketData.BidPrices.Length > 0)
            {
                FillOrder(order, marketData.BidPrices[0], order.Quantity, marketData.Timestamp);
            }
            else
            {
                FillOrder(order, marketData.Price, order.Quantity, marketData.Timestamp);
            }
        }
    }

    private void ExecuteLimitOrder(Order order, MarketData marketData)
    {
        // Check for immediate match (cross)
        if (order.Side == OrderSide.Buy)
        {
            // If Market Price <= Limit Price, we might get filled.
            // But realistically, we look at the ticks (trades) that happened.
            // Spec SC-002: "Verified against historical Tick data... whether cumulated volume exceeds queue"
            
            // 1. Check if price condition met by current tick
            if (marketData.Price <= order.Price)
            {
                // The trade happened at a price we are willing to buy.
                // Did we clear the queue?
                
                // Reduce queue by volume traded in this tick
                order.QueuePositionVolume -= marketData.Volume;

                if (order.QueuePositionVolume < 0)
                {
                    // Our turn!
                    // Volume available for us is -QueuePositionVolume
                    // (Simplified: we assume we get filled if the queue is cleared)
                    
                    FillOrder(order, order.Price, order.Quantity, marketData.Timestamp);
                }
            }
        }
        else // Sell
        {
            if (marketData.Price >= order.Price)
            {
                 order.QueuePositionVolume -= marketData.Volume;

                if (order.QueuePositionVolume < 0)
                {
                    FillOrder(order, order.Price, order.Quantity, marketData.Timestamp);
                }
            }
        }
    }

    private void FillOrder(Order order, decimal price, int quantity, DateTime time)
    {
        order.Status = OrderStatus.Filled;
        order.FilledVolume = quantity;
        
        // In real system we would create Execution entity here or return it.
        // For now, we update the order object state.
        
        _logger.LogInformation("Order {Id} FILLED at {Price} qty {Qty}", order.Id, price, quantity);
    }
}
