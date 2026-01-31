using Potato.Client.External.Fugle.Models;

namespace Potato.Client.External.Fugle;

public interface IFugleApiClient
{
    Task<string> GetIntradayQuoteAsync(string symbolId);
    Task<List<SnapshotData>> GetSnapshotQuotesAsync(string market);
}
