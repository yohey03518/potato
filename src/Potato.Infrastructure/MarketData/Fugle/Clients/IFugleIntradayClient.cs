namespace Potato.Infrastructure.MarketData.Fugle.Clients;

public interface IFugleIntradayClient
{
    Task<string> GetIntradayQuoteAsync(string symbolId);
}
