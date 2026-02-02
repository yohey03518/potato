using Potato.Core.Entities;

namespace Potato.Core.Interfaces;

public interface IStrategy
{
    string Name { get; }
    TradeSignal? Evaluate(IntradayQuote quote);
}
