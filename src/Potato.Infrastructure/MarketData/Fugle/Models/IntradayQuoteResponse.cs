using System.Text.Json.Serialization;

namespace Potato.Infrastructure.MarketData.Fugle.Models;

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

    [JsonPropertyName("bids")]
    public List<OrderBookUnitModel> Bids { get; set; } = new();

    [JsonPropertyName("asks")]
    public List<OrderBookUnitModel> Asks { get; set; } = new();

    [JsonPropertyName("lastUpdated")]
    public long LastUpdated { get; set; }
}

public class OrderBookUnitModel
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("volume")]
    public long Volume { get; set; }
}
