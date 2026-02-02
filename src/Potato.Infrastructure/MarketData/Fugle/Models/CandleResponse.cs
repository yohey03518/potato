using System.Text.Json.Serialization;

namespace Potato.Infrastructure.MarketData.Fugle.Models;

public class CandleResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public List<CandleData> Data { get; set; } = new();
}

public class CandleData
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("open")]
    public decimal Open { get; set; }

    [JsonPropertyName("high")]
    public decimal High { get; set; }

    [JsonPropertyName("low")]
    public decimal Low { get; set; }

    [JsonPropertyName("close")]
    public decimal Close { get; set; }

    [JsonPropertyName("volume")]
    public long Volume { get; set; }

    [JsonPropertyName("change")]
    public decimal Change { get; set; }
}
