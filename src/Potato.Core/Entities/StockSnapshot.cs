namespace Potato.Core.Entities;

public class StockSnapshot
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? OpenPrice { get; set; }
    public decimal? HighPrice { get; set; }
    public decimal? LowPrice { get; set; }
    public decimal? ClosePrice { get; set; }
    public decimal? Change { get; set; }
    public decimal? ChangePercent { get; set; }
    public long TradeVolume { get; set; }
    public decimal TradeValue { get; set; }
    public DateOnly Date { get; set; }
    public DateTime LastUpdated { get; set; }
}
