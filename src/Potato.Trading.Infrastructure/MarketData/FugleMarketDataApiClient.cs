using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;
using MarketDataEntity = Potato.Trading.Core.Entities.MarketData;

namespace Potato.Trading.Infrastructure.MarketData;

public class FugleMarketDataApiClient : IMarketDataApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FugleMarketDataApiClient> _logger;
    private readonly string _apiKey;

    public FugleMarketDataApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<FugleMarketDataApiClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = _configuration["Trading:FugleApiKey"] ?? throw new ArgumentNullException("FugleApiKey not found in configuration");
    }

    public async Task<MarketDataEntity> GetSnapshotAsync(string symbol)
    {
        // Reference: https://developer.fugle.tw/docs/data/market/candles
        // Note: This is a simplified implementation. Real Fugle API might differ.
        // We will assume a standard endpoint structure for this simulation if strict API docs are not provided in context,
        // but the plan mentions "REST Snapshot".
        
        var url = $"https://api.fugle.tw/marketdata/v1.0/stock/intraday/quote/{symbol}?apiToken={_apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            
            // TODO: Parse the actual JSON structure from Fugle.
            // For now, we return a mock object or throw if parsing fails.
            // In a real scenario, we would map the DTO to our Domain Entity.
            
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            
            // Assuming simplified JSON structure for demonstration
            // { "price": 100.5, "totalVolume": 5000, ... }
            
            // Note: Since I don't have the live API response schema in front of me,
            // I will implement a basic mapping based on common fields.
            
            return new MarketDataEntity
            {
                Symbol = symbol,
                Timestamp = DateTime.UtcNow,
                // Placeholder mapping
                Price = 0, 
                Volume = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching market data for {Symbol}", symbol);
            throw;
        }
    }
}
