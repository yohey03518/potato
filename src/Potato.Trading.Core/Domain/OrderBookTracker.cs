using System;
using Potato.Trading.Core.Entities;

namespace Potato.Trading.Core.Domain;

public class OrderBookTracker
{
    // In a real simulation, we might maintain the full order book state.
    // For T017 & SC-002, we need to track:
    // "Given 發出限價買單... 系統記錄當前委託單在買盤中的排隊順序（根據即時五檔掛單量）"
    
    // So when we place an order, we look at the current MarketData (snapshot) 
    // and determine how much volume is ahead of us.
    
    public long CalculateQueuePosition(Order order, MarketData marketData)
    {
        if (order.Type != OrderType.Limit) return 0; // Market orders don't queue in the same way (they take liquidity)

        if (order.Side == OrderSide.Buy)
        {
            // If buying at P, we need to see how much volume is at P or higher (better for sellers) in the Bid book?
            // Actually, if we Buy at Limit P:
            // If P >= BestAsk, we match immediately (taker).
            // If P < BestAsk, we post to Bid.
            // If there are existing Bids at P, we queue behind them.
            // If P is not in the top 5 bids, we can't know the exact queue, 
            // but usually we assume we are behind all visible volume at that price.
            
            // Check if our price matches any BidPrice level
            for (int i = 0; i < marketData.BidPrices.Length; i++)
            {
                if (marketData.BidPrices[i] == order.Price)
                {
                    // We are behind this existing volume
                    return marketData.BidVolumes[i];
                }
            }
            
            // If not found in top 5, but P < BestAsk, we assume 0 ahead if it's a new price level (improving),
            // or if it's deep in the book, we might assume 0 for simulation simplicity or infinite/unknown.
            // Spec says "根據即時五檔掛單量".
            return 0; 
        }
        else // Sell
        {
            // If Selling at P:
            // If P <= BestBid, match immediately.
            // If P > BestBid, post to Ask.
            
            for (int i = 0; i < marketData.AskPrices.Length; i++)
            {
                if (marketData.AskPrices[i] == order.Price)
                {
                    return marketData.AskVolumes[i];
                }
            }
            return 0;
        }
    }
}
