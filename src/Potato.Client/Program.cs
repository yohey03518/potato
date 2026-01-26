using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Potato.Client.External.Fugle;
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

    // Register FugleApiClient with HttpClient
    builder.Services.AddHttpClient<IFugleApiClient, FugleApiClient>(client =>
    {
        if (!string.IsNullOrEmpty(fugleConfig.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", fugleConfig.ApiKey);
        }
    });

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