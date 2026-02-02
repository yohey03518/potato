using System.Text.Json;
using Potato.Core.Entities;
using Potato.Core.Interfaces;
using Potato.Infrastructure.MarketData.Fugle.Clients;
using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle;

public class FugleMarketDataProxy(
    IFugleIntradayClient intradayClient, 
    IFugleSnapshotClient snapshotClient,
    IFugleTechnicalClient technicalClient,
    IFugleHistoryClient historyClient)
    : IMarketDataProxy
{
    public async Task<List<Candle>> GetTechnicalCandlesAsync(string symbolId, string from, string to)
    {
        var response = await historyClient.GetCandlesAsync(symbolId, from, to);

        if (response == null || response.Data == null)
            return new List<Candle>();

        return response.Data.Select(d => 
        {
            DateOnly.TryParse(d.Date, out var date);
            return new Candle
            {
                Date = date,
                Open = d.Open,
                High = d.High,
                Low = d.Low,
                Close = d.Close,
                Volume = d.Volume,
                Change = d.Change
            };
        }).ToList();
    }
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
        var response = await snapshotClient.GetSnapshotQuotesAsync(market);
        
        if (response == null || response.Data == null)
            return new List<StockSnapshot>();

        DateOnly snapshotDate;
        if (!DateOnly.TryParseExact(response.Date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out snapshotDate))
        {
            // Fallback or log error? If API date format changes. 
            // The example shows "2023-05-29".
            snapshotDate = DateOnly.FromDateTime(DateTime.Today);
        }

        return response.Data.Select(d => new StockSnapshot
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
            Date = snapshotDate,
            LastUpdated = DateTime.Now 
        }).ToList();
    }

    public async Task<List<SmaData>> GetTechnicalSmaAsync(string symbolId, int period, string from, string to)
    {
        var response = await technicalClient.GetSmaAsync(symbolId, period, from, to);
        
        if (response == null || response.Data == null) 
            return new List<SmaData>();

        return response.Data.Select(d => new SmaData
        {
            Date = DateOnly.Parse(d.Date),
            Value = d.Sma
        }).ToList();
    }
}