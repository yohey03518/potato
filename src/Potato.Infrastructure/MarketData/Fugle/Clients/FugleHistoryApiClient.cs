using System.Text.Json;
using Microsoft.Extensions.Logging;
using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public class FugleHistoryApiClient(HttpClient httpClient, ILogger<FugleHistoryApiClient> logger)
    : IFugleHistoryClient
{
    public async Task<CandleResponse?> GetCandlesAsync(string symbol, string from, string to)
    {
        var url = $"https://api.fugle.tw/marketdata/v1.0/stock/historical/candles/{symbol}?from={from}&to={to}&fields=open,high,low,close,volume,change";

        logger.LogInformation("Fetching historical candles from Fugle API for {Symbol} (From: {From}, To: {To})", symbol, from, to);

        try
        {
            var response = await httpClient.GetAsync(url);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                logger.LogWarning("Historical API is forbidden (403). API Key may not have sufficient permissions.");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Fugle API call failed with status code: {StatusCode}, Body: {Body}", response.StatusCode, await response.Content.ReadAsStringAsync());
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var candleResponse = JsonSerializer.Deserialize<CandleResponse>(content);
            
            return candleResponse;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching historical candles for {Symbol}", symbol);
            return null;
        }
    }
}
