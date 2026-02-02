namespace Potato.Core.Entities;

public enum TradeAction
{
    Buy,
    Sell
}

public class TradeSignal
{
    public string Symbol { get; set; } = string.Empty;
    public TradeAction Action { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string StrategyName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
