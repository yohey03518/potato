using System.Text.Json.Serialization;

namespace Potato.Client.External.Fugle.Models;

public class IntradayQuoteResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("lastPrice")]
    public decimal? LastPrice { get; set; }

    [JsonPropertyName("change")]
    public decimal? Change { get; set; }

    [JsonPropertyName("changePercent")]
    public decimal? ChangePercent { get; set; }

    [JsonPropertyName("lastUpdated")]
    public long LastUpdated { get; set; }
}
