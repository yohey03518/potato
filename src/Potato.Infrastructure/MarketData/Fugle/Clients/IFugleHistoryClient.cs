using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public interface IFugleHistoryClient
{
    Task<CandleResponse?> GetCandlesAsync(string symbol, string from, string to);
}
