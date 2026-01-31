namespace Potato.Client.Domain.Models;

public class IntradayQuote
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? LastPrice { get; set; }
    public decimal? Change { get; set; }
    public decimal? ChangePercent { get; set; }
    public long LastUpdated { get; set; }
}
