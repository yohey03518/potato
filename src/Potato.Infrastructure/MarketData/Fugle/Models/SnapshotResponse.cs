using System.Text.Json.Serialization;

namespace Potato.Infrastructure.MarketData.Fugle.Models;

public class SnapshotResponse
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("market")]
    public string Market { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public List<SnapshotData> Data { get; set; } = new();
}

public class SnapshotData
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("openPrice")]
    public decimal? OpenPrice { get; set; }

    [JsonPropertyName("highPrice")]
    public decimal? HighPrice { get; set; }

    [JsonPropertyName("lowPrice")]
    public decimal? LowPrice { get; set; }

    [JsonPropertyName("closePrice")]
    public decimal? ClosePrice { get; set; }

    [JsonPropertyName("change")]
    public decimal? Change { get; set; }

    [JsonPropertyName("changePercent")]
    public decimal? ChangePercent { get; set; }

    [JsonPropertyName("tradeVolume")]
    public long TradeVolume { get; set; }

    [JsonPropertyName("tradeValue")]
    public decimal TradeValue { get; set; }

    [JsonPropertyName("lastUpdated")]
    public long LastUpdated { get; set; }
}
