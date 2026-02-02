using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Potato.Client.Services;
using Potato.Core.Entities;
using Potato.Core.Interfaces;

namespace Potato.Client.Tests.Services;

[TestFixture]
public class MarketScanCandidateProviderTests
{
    private IMarketDataProxy _marketDataProxy = null!;
    private ILogger<MarketScanCandidateProvider> _logger = null!;
    private MarketScanCandidateProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _marketDataProxy = Substitute.For<IMarketDataProxy>();
        _logger = Substitute.For<ILogger<MarketScanCandidateProvider>>();
        _provider = new MarketScanCandidateProvider(_marketDataProxy, _logger);
    }

    [Test]
    public async Task GetAsync_ShouldReturnEmpty_WhenNoCandlesReturned()
    {
        // Arrange
        var stock = new StockSnapshot { Symbol = "2330" };
        _marketDataProxy.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { stock });
        _marketDataProxy.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());
        
        _marketDataProxy.GetTechnicalCandlesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(new List<Candle>());

        // Act
        var result = await _provider.GetAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_ShouldReturnEmpty_WhenVolumeBelowThreshold()
    {
        // Arrange
        var stock = new StockSnapshot { Symbol = "2330" };
        _marketDataProxy.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { stock });
        _marketDataProxy.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());

        // Generate candles with low volume
        var candles = GenerateCandles(30, close: 100, volume: 4000);
        _marketDataProxy.GetTechnicalCandlesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(candles);

        // Act
        var result = await _provider.GetAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_ShouldReturnEmpty_WhenPriceBelowMA()
    {
        // Arrange
        var stock = new StockSnapshot { Symbol = "2330" };
        _marketDataProxy.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { stock });
        _marketDataProxy.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());

        // Generate candles where Close (100) < SMA.
        // If all closes are 100, SMA is 100. Price is 100.
        // Current logic: Price > SMA. 100 > 100 is False.
        var candles = GenerateCandles(30, close: 100, volume: 6000);
        _marketDataProxy.GetTechnicalCandlesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(candles);

        // Act
        var result = await _provider.GetAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_ShouldReturnCandidate_WhenConditionsMet()
    {
        // Arrange
        var stock = new StockSnapshot { Symbol = "2330" };
        _marketDataProxy.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { stock });
        _marketDataProxy.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());

        // Generate candles where Last Close > SMA
        // Past 20 days Close = 100. SMA = 100.
        // Last Candle Close = 110.
        // Last Candle Volume = 6000.
        
        var candles = GenerateCandles(25, close: 100, volume: 6000);
        // Modify last candle to have higher price
        var lastCandle = candles.Last();
        lastCandle.Close = 110; 
        // Note: SMA calculation usually includes the current candle if we just take last 20.
        // Logic: Skip(targetIndex - 19).Take(20).
        // If last candle changes, SMA changes slightly.
        // (19 * 100 + 110) / 20 = (1900 + 110)/20 = 2010/20 = 100.5
        // Price 110 > SMA 100.5 -> True.

        _marketDataProxy.GetTechnicalCandlesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(candles);

        // Act
        var result = await _provider.GetAsync();

        // Assert
        result.Should().ContainSingle();
        result.First().Symbol.Should().Be("2330");
    }

    private List<Candle> GenerateCandles(int count, decimal close, long volume)
    {
        var candles = new List<Candle>();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-count));
        
        for (int i = 0; i < count; i++)
        {
            // Skip "Today" to ensure we simulate "Yesterday's" data being the target
            // OR ensure GenerateCandles creates data UP TO Yesterday.
            // My logic in filter: "LastOrDefault(c => c.Date < today)".
            // So if I generate candles up to Yesterday, it picks the last one.
            
            candles.Add(new Candle
            {
                Date = date.AddDays(i),
                Close = close,
                Volume = volume,
                High = close + 5,
                Low = close - 5,
                Open = close
            });
        }
        return candles;
    }
}
