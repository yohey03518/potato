using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;
using Potato.Trading.Infrastructure.Data;

namespace Potato.Trading.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly TradingDbContext _dbContext;

    public OrderRepository(TradingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Orders.FindAsync(id);
    }

    public async Task<List<Order>> GetBySymbolAsync(string symbol)
    {
        return await _dbContext.Orders
            .Where(o => o.Symbol == symbol)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
    }
}
