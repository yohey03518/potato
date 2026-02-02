using System.Text.Json;
using Microsoft.Extensions.Logging;
using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public class FugleTechnicalApiClient(HttpClient httpClient, ILogger<FugleTechnicalApiClient> logger)
    : IFugleTechnicalClient
{
    public async Task<SmaResponse?> GetSmaAsync(string symbol, int period, string from, string to)
    {
        // Construct the URL based on the documentation:
        // GET /technical/sma/{symbol}?from={from}&to={to}&timeframe=D&period={period}
        var url = $"https://api.fugle.tw/marketdata/v1.0/stock/technical/sma/{symbol}?from={from}&to={to}&timeframe=D&period={period}";

        logger.LogInformation("Fetching SMA from Fugle API for {Symbol} (Period: {Period}, From: {From}, To: {To})", symbol, period, from, to);

        try
        {
            var response = await httpClient.GetAsync(url);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                logger.LogWarning("Technical API is forbidden (403). API Key may not have sufficient permissions.");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Fugle API call failed with status code: {StatusCode}, Body: {Body}", response.StatusCode, await response.Content.ReadAsStringAsync());
                // We might choose to return null instead of throwing to avoid crashing the whole cycle for one stock
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var smaResponse = JsonSerializer.Deserialize<SmaResponse>(content);
            
            return smaResponse;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching SMA for {Symbol}", symbol);
            return null;
        }
    }
}
