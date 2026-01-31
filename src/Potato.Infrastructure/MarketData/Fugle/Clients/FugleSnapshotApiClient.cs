using System.Text.Json;
using Microsoft.Extensions.Logging;
using Potato.Infrastructure.MarketData.Fugle.Clients;
using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public class FugleSnapshotApiClient : IFugleSnapshotClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FugleSnapshotApiClient> _logger;

    public FugleSnapshotApiClient(HttpClient httpClient, ILogger<FugleSnapshotApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<SnapshotData>> GetSnapshotQuotesAsync(string market)
    {
        var url = $"https://api.fugle.tw/marketdata/v1.0/stock/snapshot/quotes/{market}?type=COMMONSTOCK";

        _logger.LogInformation("Fetching snapshot quotes from Fugle API for market: {Market}", market);

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogWarning("Snapshot Quotes API is forbidden (403). API Key does not have sufficient permissions.");
                Console.WriteLine("\n[API Permission Error] Unable to fetch stock list for filtering.");
                Console.WriteLine("The 'Snapshot Quotes' API requires a Developer or Advanced plan.");
                Console.WriteLine("Please upgrade your Fugle API key to use this feature.\n");
                return new List<SnapshotData>();
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Fugle API call failed with status code: {StatusCode}, Body: {Body}", response.StatusCode, await response.Content.ReadAsStringAsync());
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            var snapshotResponse = JsonSerializer.Deserialize<SnapshotResponse>(content);

            return snapshotResponse?.Data ?? new List<SnapshotData>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for market: {Market}", market);
            throw;
        }
    }
}
