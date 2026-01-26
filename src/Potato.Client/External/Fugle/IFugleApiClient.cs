namespace Potato.Client.External.Fugle;

public interface IFugleApiClient
{
    Task<string> GetIntradayQuoteAsync(string symbolId);
}
