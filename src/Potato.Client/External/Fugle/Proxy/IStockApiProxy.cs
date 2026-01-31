using Potato.Client.Domain.Models;

namespace Potato.Client.External.Fugle.Proxy;

public interface IStockApiProxy
{
    Task<IntradayQuote?> GetIntradayQuoteAsync(string symbolId);
    Task<List<StockSnapshot>> GetSnapshotQuotesAsync(string market);
}
