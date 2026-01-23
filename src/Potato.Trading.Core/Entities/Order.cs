using System;

namespace Potato.Trading.Core.Entities;

public enum OrderSide
{
    Buy,
    Sell
}

public enum OrderType
{
    Market,
    Limit
}

public enum OrderStatus
{
    Pending,
    PartialFilled,
    Filled,
    Cancelled,
    Rejected
}

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; } = string.Empty;
    public OrderSide Side { get; set; }
    public OrderType Type { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    public long QueuePositionVolume { get; set; }
    public int FilledVolume { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
