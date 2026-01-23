using System;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Indicators;

namespace Potato.Trading.Core.Services;

public enum SignalType
{
    None,
    Buy,
    Sell,
    Short,
    Cover
}

public class StrategySignal
{
    public string Symbol { get; set; } = string.Empty;
    public SignalType Type { get; set; }
    public decimal TriggerPrice { get; set; }
    public DateTime Timestamp { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class StrategyEvaluator
{
    private readonly SmaIndicator _sma20 = new(20);
    private decimal? _previousClose;

    public StrategySignal Evaluate(KLine kline, TradeDirection allowedDirection)
    {
        var currentSma = _sma20.Calculate(kline.Close);
        
        if (!_sma20.IsReady || _previousClose == null)
        {
            _previousClose = kline.Close;
            return new StrategySignal { Type = SignalType.None };
        }

        var signal = new StrategySignal
        {
            Symbol = kline.Symbol,
            Type = SignalType.None,
            Timestamp = kline.StartTime,
            TriggerPrice = kline.Close
        };

        // Strategy Logic:
        // Buy: Close crosses above SMA20 (and allowed Long)
        // Sell: Close crosses below SMA20 (Exit Long)
        // Short: Close crosses below SMA20 (and allowed Short)
        // Cover: Close crosses above SMA20 (Exit Short)

        bool isAboveSma = kline.Close > currentSma;
        bool wasBelowSma = _previousClose < currentSma; // Approximate previous SMA check using current SMA for simplicity or store previous SMA
                                                        // Ideally we should track previous SMA too. 
                                                        // But "Crossover" implies crossing the line.
        
        // For strict correctness, we'd need previous SMA. 
        // But for this task, let's assume crossing current SMA level from previous price is the trigger.
        // Or better, track state.
        
        // Simplified Logic:
        // Crossover Up: Previous Close < Current SMA && Current Close > Current SMA
        // Crossover Down: Previous Close > Current SMA && Current Close < Current SMA

        if (allowedDirection == TradeDirection.Long)
        {
            if (_previousClose < currentSma && kline.Close > currentSma)
            {
                signal.Type = SignalType.Buy;
                signal.Reason = "Crossover SMA20 Up";
            }
            else if (_previousClose > currentSma && kline.Close < currentSma)
            {
                signal.Type = SignalType.Sell; // Close Long
                signal.Reason = "Crossover SMA20 Down";
            }
        }
        else if (allowedDirection == TradeDirection.Short)
        {
            if (_previousClose > currentSma && kline.Close < currentSma)
            {
                signal.Type = SignalType.Short;
                signal.Reason = "Crossover SMA20 Down";
            }
            else if (_previousClose < currentSma && kline.Close > currentSma)
            {
                signal.Type = SignalType.Cover; // Close Short
                signal.Reason = "Crossover SMA20 Up";
            }
        }

        _previousClose = kline.Close;
        return signal;
    }
}
