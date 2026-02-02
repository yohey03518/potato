using Potato.Core.Entities;

namespace Potato.Core.Interfaces;

public interface IMarketDataProxy
{
    Task<IntradayQuote?> GetIntradayQuoteAsync(string symbolId);
    Task<List<StockSnapshot>> GetSnapshotQuotesAsync(string market);
    Task<List<SmaData>> GetTechnicalSmaAsync(string symbolId, int period, string from, string to);
}
