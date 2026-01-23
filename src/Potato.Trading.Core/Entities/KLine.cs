using System;

namespace Potato.Trading.Core.Entities;

public class KLine
{
    public string Symbol { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
    public decimal SMA20 { get; set; }
}
