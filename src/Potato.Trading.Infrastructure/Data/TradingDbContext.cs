using Microsoft.EntityFrameworkCore;
using Potato.Trading.Core.Entities;
using MarketDataEntity = Potato.Trading.Core.Entities.MarketData;

namespace Potato.Trading.Infrastructure.Data;

public class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options)
    {
    }

    public DbSet<Watchlist> Watchlists { get; set; } = null!;
    public DbSet<MarketDataEntity> MarketData { get; set; } = null!;
    public DbSet<KLine> KLines { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Execution> Executions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Watchlist Configuration
        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BasePrice).HasPrecision(18, 4);
            entity.Property(e => e.MA20_Day).HasPrecision(18, 4);
            entity.HasIndex(e => new { e.Symbol, e.TradeDate }).IsUnique();
        });

        // MarketData Configuration (Snapshot)
        modelBuilder.Entity<MarketDataEntity>(entity =>
        {
            entity.HasNoKey(); // MarketData is a transient snapshot, but if we store it, we might need a key. 
                               // For now, based on requirements, it seems to be event data.
                               // However, EF Core requires a key for entities in DbSet.
                               // If it's just for streaming/processing, it might not need to be in DbContext as a DbSet unless we persist tick history.
                               // The spec implies persisting trade history (Order/Execution).
                               // Let's assume for now we might persist Ticks for replay, so we'll give it a composite key or ID.
                               // But FR-009 only mentions Signal, Order, Execution.
                               // I will leave it as Keyless for now if it's not strictly persisted via EF, 
                               // but to use DbSet it needs a Key. I'll add a composite key or a surrogate key if needed later.
                               // For now, I'll assume we might not persist *every* tick to MySQL via EF in this exact structure efficiently.
                               // But to satisfy the requirement of "Infrastructure/Data", I will define it.
                               // Let's add a surrogate key to make EF happy if we were to persist it.
            entity.ToTable("MarketTicks");
            entity.Property<Guid>("Id").HasValueGenerator<Microsoft.EntityFrameworkCore.ValueGeneration.GuidValueGenerator>();
            entity.HasKey("Id");

            entity.Property(e => e.Price).HasPrecision(18, 4);
            // Storing arrays in relational DB is tricky. For now, we might serialize them or ignore them.
            // Simplified for EF:
            entity.Ignore(e => e.BidPrices);
            entity.Ignore(e => e.BidVolumes);
            entity.Ignore(e => e.AskPrices);
            entity.Ignore(e => e.AskVolumes);
        });

        // KLine Configuration
        modelBuilder.Entity<KLine>(entity =>
        {
            entity.HasKey(e => new { e.Symbol, e.StartTime });
            entity.Property(e => e.Open).HasPrecision(18, 4);
            entity.Property(e => e.High).HasPrecision(18, 4);
            entity.Property(e => e.Low).HasPrecision(18, 4);
            entity.Property(e => e.Close).HasPrecision(18, 4);
            entity.Property(e => e.SMA20).HasPrecision(18, 4);
        });

        // Order Configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 4);
            entity.Property(e => e.Side).HasConversion<string>();
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
        });

        // Execution Configuration
        modelBuilder.Entity<Execution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 4);
            entity.Property(e => e.Fee).HasPrecision(18, 4);
            entity.Property(e => e.Tax).HasPrecision(18, 4);
        });
    }
}
