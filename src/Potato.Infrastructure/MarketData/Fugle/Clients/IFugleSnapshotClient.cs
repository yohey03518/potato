using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public interface IFugleSnapshotClient
{
    Task<List<SnapshotData>> GetSnapshotQuotesAsync(string market);
}
