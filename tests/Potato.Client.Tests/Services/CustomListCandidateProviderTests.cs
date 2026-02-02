using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Potato.Client.Services;
using Potato.Core.Entities;
using Potato.Core.Interfaces;

namespace Potato.Client.Tests.Services;

[TestFixture]
public class CustomListCandidateProviderTests
{
    private IConfiguration _configuration = null!;
    private ILogger<CustomListCandidateProvider> _logger = null!;
    private CustomListCandidateProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<CustomListCandidateProvider>>();
    }

    private void SetupConfiguration(List<string> symbols)
    {
        var inMemorySettings = new Dictionary<string, string?>();
        for (int i = 0; i < symbols.Count; i++)
        {
            inMemorySettings[$"StockSettings:CustomCandidates:{i}"] = symbols[i];
        }

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
            
        _provider = new CustomListCandidateProvider(_configuration, _logger);
    }

    [Test]
    public async Task GetAsync_ShouldReturnSkeletonSnapshots_WhenCustomCandidatesAreConfigured()
    {
        // Arrange
        var symbols = new List<string> { "2330", "2317" };
        SetupConfiguration(symbols);

        // Act
        var result = await _provider.GetAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Select(s => s.Symbol).Should().Contain(new[] { "2330", "2317" });
        result.All(s => s.Date == DateOnly.FromDateTime(DateTime.Today)).Should().BeTrue();
    }

    [Test]
    public async Task GetAsync_ShouldReturnEmpty_WhenNoCustomCandidatesInConfig()
    {
        // Arrange
        SetupConfiguration(new List<string>());

        // Act
        var result = await _provider.GetAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
