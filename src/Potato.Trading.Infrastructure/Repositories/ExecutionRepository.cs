using System.Threading.Tasks;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;
using Potato.Trading.Infrastructure.Data;

namespace Potato.Trading.Infrastructure.Repositories;

public class ExecutionRepository : IExecutionRepository
{
    private readonly TradingDbContext _dbContext;

    public ExecutionRepository(TradingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Execution execution)
    {
        await _dbContext.Executions.AddAsync(execution);
        await _dbContext.SaveChangesAsync();
    }
}
