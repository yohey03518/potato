using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Potato.Trading.Core.Entities;

namespace Potato.Trading.Core.Interfaces;

public interface IWatchlistRepository
{
    Task<Watchlist?> GetBySymbolAsync(string symbol, DateTime date);
    Task<List<Watchlist>> GetByDateAsync(DateTime date);
    Task AddAsync(Watchlist watchlist);
}
