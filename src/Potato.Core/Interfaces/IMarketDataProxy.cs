using Potato.Core.Entities;

namespace Potato.Core.Interfaces;

public interface IMarketDataProxy
{
    Task<IntradayQuote?> GetIntradayQuoteAsync(string symbolId);
    Task<List<StockSnapshot>> GetSnapshotQuotesAsync(string market);
}
