using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Potato.Trading.Infrastructure.Data;
using Potato.Trading.Core.Interfaces;
using Potato.Trading.Core.Services;
using Potato.Trading.Infrastructure.Repositories;
using Potato.Trading.Infrastructure.MarketData;

namespace Potato.Trading.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Potato Trading Simulator Started.");

        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<TradingDbContext>(options =>
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

                services.AddHttpClient<FugleMarketDataApiClient>();
                services.AddScoped<IMarketDataApiClient, FugleMarketDataApiClient>();
                services.AddSingleton<IMarketDataStreamClient, FugleWebSocketClient>();
                
                services.AddScoped<IWatchlistRepository, WatchlistRepository>();
                services.AddScoped<IOrderRepository, OrderRepository>();
                services.AddScoped<IExecutionRepository, ExecutionRepository>();
                
                services.AddScoped<MarketClock>();
                services.AddScoped<TradingService>(); 
                services.AddScoped<MatchingEngine>();
                services.AddScoped<LiquidationService>();
                services.AddScoped<IMarketDataService, MarketDataService>();

                services.AddHostedService<Worker>();
            });
}
