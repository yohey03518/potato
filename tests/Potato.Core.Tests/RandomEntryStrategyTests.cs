using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Potato.Core.Entities;
using Potato.Core.Services;

namespace Potato.Core.Tests;

public class RandomEntryStrategyTests
{
    private IConfiguration CreateConfig(bool enabled, int probability)
    {
        var settings = new Dictionary<string, string?>
        {
            {"Strategies:RandomEntry:Enabled", enabled.ToString().ToLower()},
            {"Strategies:RandomEntry:ProbabilityPercent", probability.ToString()}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Test]
    public void Evaluate_ShouldReturnSignal_WhenProbabilityIs100()
    {
        // Arrange
        var config = CreateConfig(true, 100);
        var strategy = new RandomEntryStrategy(config);
        var quote = new IntradayQuote
        {
            Symbol = "2330",
            Bids = new List<OrderBookUnit>
            {
                new(600, 10),
                new(599, 20) // Bid 2
            }
        };

        // Act
        var result = strategy.Evaluate(quote);

        // Assert
        result.Should().NotBeNull();
        result!.Symbol.Should().Be("2330");
        result.Price.Should().Be(599); // Should be Bid 2
        result.Action.Should().Be(TradeAction.Buy);
    }

    [Test]
    public void Evaluate_ShouldReturnNull_WhenDisabled()
    {
        // Arrange
        var config = CreateConfig(false, 100);
        var strategy = new RandomEntryStrategy(config);
        var quote = new IntradayQuote { Symbol = "2330", Bids = new List<OrderBookUnit> { new(100, 1), new(99, 1) } };

        // Act
        var result = strategy.Evaluate(quote);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Evaluate_ShouldReturnNull_WhenBidsAreInsufficient()
    {
        // Arrange
        var config = CreateConfig(true, 100);
        var strategy = new RandomEntryStrategy(config);
        var quote = new IntradayQuote { Symbol = "2330", Bids = new List<OrderBookUnit> { new(100, 1) } };

        // Act
        var result = strategy.Evaluate(quote);

        // Assert
        result.Should().BeNull();
    }
}
