using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Potato.Trading.Core.Entities;

namespace Potato.Trading.Core.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<List<Order>> GetBySymbolAsync(string symbol);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}
