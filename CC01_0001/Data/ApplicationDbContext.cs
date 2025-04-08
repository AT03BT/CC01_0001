/*
    Application/ApplicationDbContext.cs
    Version: 0.1.0
    (c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
    https://creativecommons.org/licenses/by/4.0/
*/

using CC01_0001.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;

namespace CC01_0001.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<BinanceExchangeRateLimits> BinanceExchangeRateLimits { get; set; }
    public DbSet<SymbolOrderType> SymbolOrderTypes { get; set; }

    public DbSet<ExchangeUpdate> ExchangeUpdates { get; set; }
    public DbSet<ExchangeInfo> ExchangeInfos { get; set; }
    public DbSet<MarketSetup> MarketSetups { get; set; }
    public DbSet<RateLimit> RateLimits { get; set; }
    public DbSet<ExchangeFilter> ExchangeFilters { get; set; }
    public DbSet<OrderType> OrderTypes { get; set; }
    public DbSet<Filter> Filters { get; set; }
    public DbSet<PermissionSetSpacer> PermissionSetSpacers { get; set; }
    public DbSet<PermissionSet> PermissionSets { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    public DbSet<BinanceTrade> BinanceTrades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ExchangeInfo>()
            .HasOne(ei => ei.ExchangeUpdate)
            .WithOne(eu => eu.ExchangeInfo)
            .HasForeignKey<ExchangeInfo>(ei => ei.ExchangeUpdateId);

        modelBuilder.Entity<MarketSetup>()
            .HasOne(ms => ms.ExchangeInfo)
            .WithMany(ei => ei.MarketSetups)
            .HasForeignKey(ms => ms.ExchangeInfoId);

        modelBuilder.Entity<PermissionSet>()
            .HasOne(p => p.PermissionSetSpacer)
            .WithMany(ms => ms.PermissionSets)
            .HasForeignKey(p => p.PermissionSetSpacerId);

        modelBuilder.Entity<Permission>()
            .HasOne(p => p.PermissionSet)
            .WithMany(ms => ms.Permissions)
            .HasForeignKey(p => p.PermissionSetId);

        // One-to-many relationships
        // =========================
        modelBuilder.Entity<ExchangeFilter>()
            .HasOne(ef => ef.ExchangeInfo)
            .WithMany(bei => bei.ExchangeFilters)
            .HasForeignKey(ef => ef.BinanceExchangeInfoId);

        modelBuilder.Entity<Filter>()
            .HasOne(f => f.Symbol)
            .WithMany(s => s.Filters)
            .HasForeignKey(f => f.SymbolId);

        // Many-to-many relationship
        modelBuilder.Entity<BinanceExchangeRateLimits>()
            .HasKey(rl => new { rl.BinanceExchangeId, rl.RateLimitId });

        modelBuilder.Entity<BinanceExchangeRateLimits>()
            .HasOne(bx => bx.BinanceExchange)
            .WithMany(bx => bx.BinanceExchangeRateLimits)
            .HasForeignKey(bx => bx.BinanceExchangeId);

        modelBuilder.Entity<BinanceExchangeRateLimits>()
            .HasOne(rl => rl.RateLimit)
            .WithMany(rl => rl.BinanceExchangeRateLimits)
            .HasForeignKey(rl => rl.RateLimitId);

        modelBuilder.Entity<SymbolOrderType>()
            .HasKey(ot => new { ot.MarketSetupId, ot.OrderTypeId }); // Composite key

        modelBuilder.Entity<SymbolOrderType>()
            .HasOne(ot => ot.marketSetup)
            .WithMany(s => s.SymbolOrderTypes)
            .HasForeignKey(ot => ot.MarketSetupId);

        modelBuilder.Entity<SymbolOrderType>()
            .HasOne(ot => ot.OrderType)
            .WithMany(o => o.SymbolOrderTypes)
            .HasForeignKey(ot => ot.OrderTypeId);
    }
}