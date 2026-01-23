using System;

namespace Potato.Trading.Core.Entities;

public class Execution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public decimal Fee { get; set; }
    public decimal Tax { get; set; }
}
