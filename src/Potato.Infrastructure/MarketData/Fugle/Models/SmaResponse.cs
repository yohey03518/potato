using System.Text.Json.Serialization;

namespace Potato.Infrastructure.MarketData.Fugle.Models;

public class SmaResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;

    [JsonPropertyName("timeframe")]
    public string Timeframe { get; set; } = string.Empty;

    [JsonPropertyName("period")]
    public int Period { get; set; }

    [JsonPropertyName("data")]
    public List<SmaDataPoint> Data { get; set; } = new();
}

public class SmaDataPoint
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("sma")]
    public decimal Sma { get; set; }
}
