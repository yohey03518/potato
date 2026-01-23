using System;

namespace Potato.Trading.Core.Entities;

public enum TradeDirection
{
    None,
    Long,
    Short
}

public class Watchlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime TradeDate { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public TradeDirection Direction { get; set; }
    public decimal BasePrice { get; set; }
    public decimal MA20_Day { get; set; }
}
