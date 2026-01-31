using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Potato.Client.External.Fugle;
using Potato.Client.External.Fugle.Clients;
using Potato.Client.External.Fugle.Proxy;
using Potato.Client.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Potato.Client...");

    var builder = Host.CreateApplicationBuilder(args);

    // Configure Serilog
    builder.Services.AddSerilog();

    // Configure FugleApiClientConfig
    var fugleConfig = new FugleApiClientConfig();
    builder.Configuration.GetSection("FugleApi").Bind(fugleConfig);
    builder.Services.AddSingleton(fugleConfig);

    // 1. Register Intraday Client (Always Real)
    builder.Services.AddHttpClient<IFugleIntradayClient, FugleIntradayApiClient>(client =>
    {
        if (!string.IsNullOrEmpty(fugleConfig.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", fugleConfig.ApiKey);
        }
    });

    // 2. Register Snapshot Client (Real or Mock based on config)
    bool useMockSnapshot = builder.Configuration.GetValue<bool>("FugleApi:UseMockSnapshot");
    
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

    // 3. Register Proxy
    builder.Services.AddSingleton<IStockApiProxy, FugleApiProxy>();

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