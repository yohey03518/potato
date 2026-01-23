using System;
using NUnit.Framework;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Services;

namespace Potato.Trading.UnitTests.Services;

[TestFixture]
public class StrategyEvaluatorTests
{
    private StrategyEvaluator _evaluator;

    [SetUp]
    public void Setup()
    {
        _evaluator = new StrategyEvaluator();
    }

    [Test]
    public void Evaluate_ShouldReturnNone_WhenNotEnoughDataForSMA()
    {
        var kline = new KLine { Close = 100, StartTime = DateTime.UtcNow };
        var signal = _evaluator.Evaluate(kline, TradeDirection.Long);
        
        Assert.That(signal.Type, Is.EqualTo(SignalType.None));
    }

    [Test]
    public void Evaluate_ShouldReturnBuy_WhenCrossoverUp_LongDirection()
    {
        // 1. Fill SMA with 19 values of 100
        for (int i = 0; i < 19; i++)
        {
            _evaluator.Evaluate(new KLine { Close = 100, StartTime = DateTime.UtcNow.AddMinutes(i * 5) }, TradeDirection.Long);
        }

        // 2. Previous Close: 98 (Below SMA 100)
        _evaluator.Evaluate(new KLine { Close = 98, StartTime = DateTime.UtcNow.AddMinutes(19 * 5) }, TradeDirection.Long);
        
        // SMA is roughly 100. Previous Close was 98.
        
        // 3. Current Close: 102 (Above SMA 100) -> Crossover Up
        var signal = _evaluator.Evaluate(new KLine { Close = 102, StartTime = DateTime.UtcNow.AddMinutes(20 * 5) }, TradeDirection.Long);

        Assert.That(signal.Type, Is.EqualTo(SignalType.Buy));
    }

    [Test]
    public void Evaluate_ShouldReturnSell_WhenCrossoverDown_LongDirection()
    {
        // Setup initial state: Price above SMA
        for (int i = 0; i < 19; i++)
        {
            _evaluator.Evaluate(new KLine { Close = 100, StartTime = DateTime.UtcNow.AddMinutes(i * 5) }, TradeDirection.Long);
        }

        // Previous Close: 102 (Above SMA)
        _evaluator.Evaluate(new KLine { Close = 102, StartTime = DateTime.UtcNow }, TradeDirection.Long);

        // Current Close: 98 (Below SMA) -> Crossover Down (Sell/Exit)
        var signal = _evaluator.Evaluate(new KLine { Close = 98, StartTime = DateTime.UtcNow }, TradeDirection.Long);

        Assert.That(signal.Type, Is.EqualTo(SignalType.Sell));
    }
}
