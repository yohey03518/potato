using System.Text.Json;
using Microsoft.Extensions.Logging;
using Potato.Infrastructure.MarketData.Fugle.Clients;

namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public class FugleIntradayApiClient : IFugleIntradayClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FugleIntradayApiClient> _logger;

    public FugleIntradayApiClient(HttpClient httpClient, ILogger<FugleIntradayApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetIntradayQuoteAsync(string symbolId)
    {
        var url = $"https://api.fugle.tw/marketdata/v1.0/stock/intraday/quote/{symbolId}";
        
        _logger.LogInformation("Fetching intraday quote from Fugle API for symbol: {SymbolId}", symbolId);
        
        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Fugle API call failed with status code: {StatusCode}, Body: {Body}", response.StatusCode, await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
        }

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}
