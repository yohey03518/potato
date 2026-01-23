using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;
using Potato.Trading.Infrastructure.Data;

namespace Potato.Trading.Infrastructure.Repositories;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly TradingDbContext _dbContext;

    public WatchlistRepository(TradingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Watchlist?> GetBySymbolAsync(string symbol, DateTime date)
    {
        return await _dbContext.Watchlists
            .FirstOrDefaultAsync(w => w.Symbol == symbol && w.TradeDate == date);
    }

    public async Task<List<Watchlist>> GetByDateAsync(DateTime date)
    {
        return await _dbContext.Watchlists
            .Where(w => w.TradeDate == date)
            .ToListAsync();
    }

    public async Task AddAsync(Watchlist watchlist)
    {
        await _dbContext.Watchlists.AddAsync(watchlist);
        await _dbContext.SaveChangesAsync();
    }
}
