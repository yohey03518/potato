using System.Text.Json;
using Potato.Client.Domain.Models;
using Potato.Client.External.Fugle.Clients;
using Potato.Client.External.Fugle.Models;

namespace Potato.Client.External.Fugle.Proxy;

public class FugleApiProxy : IStockApiProxy
{
    private readonly IFugleIntradayClient _intradayClient;
    private readonly IFugleSnapshotClient _snapshotClient;

    public FugleApiProxy(IFugleIntradayClient intradayClient, IFugleSnapshotClient snapshotClient)
    {
        _intradayClient = intradayClient;
        _snapshotClient = snapshotClient;
    }

    public async Task<IntradayQuote?> GetIntradayQuoteAsync(string symbolId)
    {
        var json = await _intradayClient.GetIntradayQuoteAsync(symbolId);
        var response = JsonSerializer.Deserialize<IntradayQuoteResponse>(json);

        if (response == null) return null;

        return new IntradayQuote
        {
            Symbol = response.Symbol,
            Name = response.Name,
            LastPrice = response.LastPrice,
            Change = response.Change,
            ChangePercent = response.ChangePercent,
            LastUpdated = response.LastUpdated
        };
    }

    public async Task<List<StockSnapshot>> GetSnapshotQuotesAsync(string market)
    {
        var fugleData = await _snapshotClient.GetSnapshotQuotesAsync(market);

        return fugleData.Select(d => new StockSnapshot
        {
            Symbol = d.Symbol,
            Name = d.Name,
            OpenPrice = d.OpenPrice,
            HighPrice = d.HighPrice,
            LowPrice = d.LowPrice,
            ClosePrice = d.ClosePrice,
            Change = d.Change,
            ChangePercent = d.ChangePercent,
            TradeVolume = d.TradeVolume,
            TradeValue = d.TradeValue,
            // Convert microsecond timestamp to DateTime if needed, or keeping it simple for now
            LastUpdated = DateTime.Now // Fugle timestamp is usually microseconds, simplified for this prototype
        }).ToList();
    }
}
