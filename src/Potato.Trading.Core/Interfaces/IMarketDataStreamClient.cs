using System;
using System.Threading;
using System.Threading.Tasks;
using Potato.Trading.Core.Entities;

namespace Potato.Trading.Core.Interfaces;

public interface IMarketDataStreamClient
{
    Task ConnectAsync(string symbol, Func<MarketData, Task> onMessage, CancellationToken cancellationToken);
    Task DisconnectAsync();
}
