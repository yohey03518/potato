using System.Threading;
using System.Threading.Tasks;

namespace Potato.Trading.Core.Interfaces;

public interface IMarketDataService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}
