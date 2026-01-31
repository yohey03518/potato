using Potato.Client.External.Fugle.Models;

namespace Potato.Client.External.Fugle.Clients;

public interface IFugleSnapshotClient
{
    Task<List<SnapshotData>> GetSnapshotQuotesAsync(string market);
}
