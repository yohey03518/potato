using Potato.Infrastructure.MarketData.Fugle.Models;

namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public interface IFugleTechnicalClient
{
    Task<SmaResponse?> GetSmaAsync(string symbol, int period, string from, string to);
}
