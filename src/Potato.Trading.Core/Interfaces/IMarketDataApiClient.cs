using System.Threading.Tasks;
using Potato.Trading.Core.Entities;

namespace Potato.Trading.Core.Interfaces;

public interface IMarketDataApiClient
{
    Task<MarketData> GetSnapshotAsync(string symbol);
}
