using System.Text.Json;
using Potato.Core.Entities;
using Potato.Core.Interfaces;
using Potato.Infrastructure.MarketData.Fugle.Clients;
using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle;

public class FugleMarketDataProxy(IFugleIntradayClient intradayClient, IFugleSnapshotClient snapshotClient)
    : IMarketDataProxy
{
    public async Task<IntradayQuote?> GetIntradayQuoteAsync(string symbolId)
    {
        var json = await intradayClient.GetIntradayQuoteAsync(symbolId);
        var response = JsonSerializer.Deserialize<IntradayQuoteResponse>(json);

        if (response == null) return null;

        return new IntradayQuote
        {
            Symbol = response.Symbol,
            Name = response.Name,
            LastPrice = response.LastPrice,
            Change = response.Change,
            ChangePercent = response.ChangePercent,
            LastUpdated = response.LastUpdated,
            Bids = response.Bids?.Select(b => new OrderBookUnit(b.Price, b.Volume)).ToList() ?? new List<OrderBookUnit>(),
            Asks = response.Asks?.Select(a => new OrderBookUnit(a.Price, a.Volume)).ToList() ?? new List<OrderBookUnit>()
        };
    }

    public async Task<List<StockSnapshot>> GetSnapshotQuotesAsync(string market)
    {
        var fugleData = await snapshotClient.GetSnapshotQuotesAsync(market);

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
            LastUpdated = DateTime.Now 
        }).ToList();
    }
}