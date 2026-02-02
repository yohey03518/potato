using Microsoft.Extensions.Configuration;
using Potato.Core.Entities;
using Potato.Core.Interfaces;

namespace Potato.Core.Services;

public class RandomEntryStrategy : IStrategy
{
    private readonly int _probabilityPercent;
    private readonly bool _enabled;
    private readonly Random _random = new();

    public string Name => "RandomEntry";

    public RandomEntryStrategy(IConfiguration configuration)
    {
        var section = configuration.GetSection("Strategies:RandomEntry");
        _enabled = section.GetValue<bool>("Enabled", false);
        _probabilityPercent = section.GetValue<int>("ProbabilityPercent", 10);
    }

    public TradeSignal? Evaluate(IntradayQuote quote)
    {
        if (!_enabled) return null;

        // Requirement: "Buy 2 Price" -> Need at least 2 bids
        if (quote.Bids == null || quote.Bids.Count < 2)
            return null;

        // Random logic: 1-100
        // If probability is 10, then 1..10 <= 10 is true.
        if (_random.Next(1, 101) > _probabilityPercent)
            return null;

        var price = quote.Bids[1].Price; // Index 1 is the 2nd bid

        return new TradeSignal
        {
            Symbol = quote.Symbol,
            Action = TradeAction.Buy,
            Price = price,
            Quantity = 1000, // Fixed 1 sheet
            StrategyName = Name
        };
    }
}
