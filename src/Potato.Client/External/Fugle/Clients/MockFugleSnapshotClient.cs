using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Potato.Client.External.Fugle.Clients;
using Potato.Client.External.Fugle.Models;

namespace Potato.Client.External.Fugle;

public class MockFugleSnapshotClient : IFugleSnapshotClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MockFugleSnapshotClient> _logger;

    public MockFugleSnapshotClient(IConfiguration configuration, ILogger<MockFugleSnapshotClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<List<SnapshotData>> GetSnapshotQuotesAsync(string market)
    {
        _logger.LogInformation("Using MOCK implementation for Snapshot Quotes ({Market})", market);

        var mockDataConfig = _configuration.GetSection("MockStockData").Get<List<MockStockItem>>();
        
        if (mockDataConfig == null || !mockDataConfig.Any())
        {
             _logger.LogWarning("No mock data found in 'MockStockData' configuration.");
             return Task.FromResult(new List<SnapshotData>());
        }

        var snapshotData = mockDataConfig.Select(item => new SnapshotData
        {
            Symbol = item.Symbol,
            Name = item.Name,
            TradeVolume = item.Volume, // Mock volume
            Type = "EQUITY",
            // Other fields can be defaulted
            OpenPrice = 100,
            ClosePrice = 105,
            Change = 5,
            ChangePercent = 5.0m
        }).ToList();

        return Task.FromResult(snapshotData);
    }
    
    private class MockStockItem
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Volume { get; set; }
    }
}
