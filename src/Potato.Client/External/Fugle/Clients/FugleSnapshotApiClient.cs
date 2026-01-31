using System.Text.Json;
using Microsoft.Extensions.Logging;
using Potato.Client.External.Fugle.Clients;
using Potato.Client.External.Fugle.Models;

namespace Potato.Client.External.Fugle;

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

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Fugle API call failed with status code: {StatusCode}, Body: {Body}", response.StatusCode, await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
        }

        var content = await response.Content.ReadAsStringAsync();
        var snapshotResponse = JsonSerializer.Deserialize<SnapshotResponse>(content);

        return snapshotResponse?.Data ?? new List<SnapshotData>();
    }
}
