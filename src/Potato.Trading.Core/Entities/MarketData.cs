using System;

namespace Potato.Trading.Core.Entities;

public class MarketData
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public long Volume { get; set; }
    public decimal[] BidPrices { get; set; } = Array.Empty<decimal>();
    public long[] BidVolumes { get; set; } = Array.Empty<long>();
    public decimal[] AskPrices { get; set; } = Array.Empty<decimal>();
    public long[] AskVolumes { get; set; } = Array.Empty<long>();
    public DateTime Timestamp { get; set; }
}
