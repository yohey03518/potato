using Potato.Core.Entities;

namespace Potato.Core.Interfaces;

public interface IInitialCandidateFilter
{
    Task<List<StockSnapshot>> GetAsync(CancellationToken stoppingToken);
}
