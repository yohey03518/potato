using Microsoft.Extensions.Logging;

namespace Potato.Client.External.Fugle;

public class FugleApiClient : IFugleApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FugleApiClient> _logger;

    public FugleApiClient(HttpClient httpClient, ILogger<FugleApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetIntradayQuoteAsync(string symbolId)
    {
        // Constructing the URL for Fugle Intraday Quote API (v0.3)
        // Reference: https://developer.fugle.tw/docs/data/http-api/intraday/quote
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
