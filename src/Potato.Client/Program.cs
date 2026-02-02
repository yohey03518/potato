using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Potato.Core.Interfaces;
using Potato.Infrastructure.MarketData.Fugle;
using Potato.Infrastructure.MarketData.Fugle.Clients;
using Potato.Client.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Potato.Client...");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog();

    var fugleConfig = new FugleApiClientConfig();
    builder.Configuration.GetSection("FugleApi").Bind(fugleConfig);
    builder.Services.AddSingleton(fugleConfig);

    builder.Services.AddHttpClient<IFugleIntradayClient, FugleIntradayApiClient>(client =>
    {
        if (!string.IsNullOrEmpty(fugleConfig.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", fugleConfig.ApiKey);
        }
    });

    var useMockSnapshot = builder.Configuration.GetValue<bool>("FugleApi:UseMockSnapshot");
    
    if (useMockSnapshot)
    {
        Log.Warning("Using MockFugleSnapshotClient as per configuration.");
        builder.Services.AddSingleton<IFugleSnapshotClient, MockFugleSnapshotClient>();
    }
    else
    {
        builder.Services.AddHttpClient<IFugleSnapshotClient, FugleSnapshotApiClient>(client =>
        {
            if (!string.IsNullOrEmpty(fugleConfig.ApiKey))
            {
                client.DefaultRequestHeaders.Add("X-API-KEY", fugleConfig.ApiKey);
            }
        });

    }

    builder.Services.AddHttpClient<IFugleTechnicalClient, FugleTechnicalApiClient>(client =>
    {
        if (!string.IsNullOrEmpty(fugleConfig.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", fugleConfig.ApiKey);
        }
    });

    builder.Services.AddSingleton<IMarketDataProxy, FugleMarketDataProxy>();

    builder.Services.AddSingleton<IInitialCandidateFilter, InitialCandidateFilter>();

    builder.Services.AddSingleton<IStrategy, Potato.Core.Services.RandomEntryStrategy>();

    // Register the Hosted Service
    builder.Services.AddHostedService<StockPriceMonitorService>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}