using Potato.Core.Entities;

namespace Potato.Core.Interfaces;

public interface IInitialCandidateProvider
{
    Task<List<StockSnapshot>> GetAsync();
}
