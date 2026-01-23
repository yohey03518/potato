using System;
using System.Threading.Tasks;
using Potato.Trading.Core.Entities;

namespace Potato.Trading.Core.Interfaces;

public interface IExecutionRepository
{
    Task AddAsync(Execution execution);
}
