namespace Potato.Client.External.Fugle.Clients;

public interface IFugleIntradayClient
{
    Task<string> GetIntradayQuoteAsync(string symbolId);
}
