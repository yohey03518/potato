using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Potato.Client.Services;
using Potato.Core.Entities;
using Potato.Core.Interfaces;

namespace Potato.Client.Tests.Services;

[TestFixture]
public class InitialCandidateFilterTests
{
    private IMarketDataProxy _marketDataProxyMock;
    private ILogger<InitialCandidateFilter> _loggerMock;
    private InitialCandidateFilter _filter;

    [SetUp]
    public void Setup()
    {
        _marketDataProxyMock = Substitute.For<IMarketDataProxy>();
        _loggerMock = Substitute.For<ILogger<InitialCandidateFilter>>();
        _filter = new InitialCandidateFilter(_marketDataProxyMock, _loggerMock);
    }

    [Test]
    public async Task GetAsync_ShouldReturnEmpty_WhenVolumeBelowThreshold()
    {
        // Arrange
        var lowVolumeStock = new StockSnapshot { Symbol = "2330", TradeVolume = 4000 };
        _marketDataProxyMock.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { lowVolumeStock });
        _marketDataProxyMock.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());

        // Act
        var result = await _filter.GetAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_ShouldReturnCandidate_WhenConditionsMet_Case1_YesterdayData()
    {
        // Case 1: Snapshot Date < Today. Compare ClosePrice vs SMA(T-1)
        var today = DateOnly.FromDateTime(DateTime.Now);
        var yesterday = today.AddDays(-1);
        
        var validStock = new StockSnapshot 
        { 
            Symbol = "2330", 
            TradeVolume = 6000, 
            Date = yesterday,
            ClosePrice = 100 
        };

        _marketDataProxyMock.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { validStock });
        _marketDataProxyMock.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());

        // SMA at 90 (Price 100 > 90 -> Include)
        var smaData = new List<SmaData> { new SmaData { Date = yesterday, Value = 90 } };
        _marketDataProxyMock.GetTechnicalSmaAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(smaData);

        // Act
        var result = await _filter.GetAsync(CancellationToken.None);

        // Assert
        result.Should().ContainSingle().Which.Symbol.Should().Be("2330");
    }

    [Test]
    public async Task GetAsync_ShouldReturnCandidate_WhenConditionsMet_Case2_TodayData()
    {
        // Case 2: Snapshot Date == Today. Compare (Close - Change) vs SMA(T-1)
        var today = DateOnly.FromDateTime(DateTime.Now);
        var yesterday = today.AddDays(-1);

        var validStock = new StockSnapshot 
        { 
            Symbol = "2330", 
            TradeVolume = 6000, 
            Date = today,
            ClosePrice = 110,
            Change = 10 
            // Previous Close = 110 - 10 = 100
        };

        _marketDataProxyMock.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { validStock });
        _marketDataProxyMock.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());

        // SMA(T-1) at 90 (PrevClose 100 > 90 -> Include)
        var smaData = new List<SmaData> { new SmaData { Date = yesterday, Value = 90 } };
        _marketDataProxyMock.GetTechnicalSmaAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(smaData);

        // Act
        var result = await _filter.GetAsync(CancellationToken.None);

        // Assert
        result.Should().ContainSingle().Which.Symbol.Should().Be("2330");
    }

    [Test]
    public async Task GetAsync_ShouldFilterOut_WhenPriceBelowMA()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var yesterday = today.AddDays(-1);
        
        var stock = new StockSnapshot 
        { 
            Symbol = "2330", 
            TradeVolume = 6000, 
            Date = yesterday,
            ClosePrice = 80 
        };

        _marketDataProxyMock.GetSnapshotQuotesAsync("TSE").Returns(new List<StockSnapshot> { stock });
        _marketDataProxyMock.GetSnapshotQuotesAsync("OTC").Returns(new List<StockSnapshot>());

        // SMA at 90 (Price 80 < 90 -> Exclude)
        var smaData = new List<SmaData> { new SmaData { Date = yesterday, Value = 90 } };
        _marketDataProxyMock.GetTechnicalSmaAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(smaData);

        // Act
        var result = await _filter.GetAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
