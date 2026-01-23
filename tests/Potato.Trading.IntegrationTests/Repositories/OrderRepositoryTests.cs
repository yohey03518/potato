using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Potato.Trading.Core.Entities;
using Potato.Trading.Infrastructure.Data;
using Potato.Trading.Infrastructure.Repositories;

namespace Potato.Trading.IntegrationTests.Repositories;

[TestFixture]
public class OrderRepositoryTests
{
    private TradingDbContext _dbContext;
    private OrderRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TradingDbContext(options);
        _repository = new OrderRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task AddAsync_ShouldPersistOrder()
    {
        var order = new Order
        {
            Symbol = "2330",
            Price = 600,
            Quantity = 1000,
            Type = OrderType.Limit,
            Side = OrderSide.Buy,
            Status = OrderStatus.Pending
        };

        await _repository.AddAsync(order);

        var retrieved = await _repository.GetByIdAsync(order.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Symbol, Is.EqualTo("2330"));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateStatus()
    {
        var order = new Order
        {
            Symbol = "2330",
            Price = 600,
            Quantity = 1000,
            Type = OrderType.Limit,
            Side = OrderSide.Buy,
            Status = OrderStatus.Pending
        };

        await _repository.AddAsync(order);

        order.Status = OrderStatus.Filled;
        await _repository.UpdateAsync(order);

        var retrieved = await _repository.GetByIdAsync(order.Id);
        Assert.That(retrieved.Status, Is.EqualTo(OrderStatus.Filled));
    }
}
