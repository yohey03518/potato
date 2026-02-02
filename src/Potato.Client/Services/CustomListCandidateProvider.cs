using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Potato.Core.Entities;
using Potato.Core.Interfaces;

namespace Potato.Client.Services;

public class CustomListCandidateProvider(
    IConfiguration configuration,
    ILogger<CustomListCandidateProvider> logger)
    : IInitialCandidateProvider
{
    public Task<List<StockSnapshot>> GetAsync()
    {
        var symbols = configuration.GetSection("StockSettings:CustomCandidates").Get<List<string>>();

        if (symbols == null || symbols.Count == 0)
        {
            logger.LogWarning("No custom candidates found in configuration (StockSettings:CustomCandidates).");
            return Task.FromResult(new List<StockSnapshot>());
        }

        logger.LogInformation("Creating skeleton snapshots for custom candidates: {Symbols}", string.Join(", ", symbols));

        var candidates = symbols.Select(symbol => new StockSnapshot 
        { 
            Symbol = symbol,
            Date = DateOnly.FromDateTime(DateTime.Today),
            LastUpdated = DateTime.Now
        }).ToList();

        logger.LogInformation("Generated {Count} skeleton snapshots from configuration.", candidates.Count);

        return Task.FromResult(candidates);
    }
}
