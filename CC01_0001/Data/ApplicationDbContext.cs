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
    public DbSet<PermissionSetPermissions> PermissionSetPermissions { get; set; }


    public DbSet<UpdateInterval> UpdateIntervals { get; set; }
    public DbSet<ExchangeInfo> ExchangeInfos { get; set; }
    public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
    public DbSet<CurrencySymbol> CurrencySymbols { get; set; }
    public DbSet<MarketSettings> MarketSettingsSet { get; set; }
    public DbSet<RateLimit> RateLimits { get; set; }
    public DbSet<ExchangeFilter> ExchangeFilters { get; set; }
    public DbSet<OrderType> OrderTypes { get; set; }
    public DbSet<Filter> Filters { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionSet> PermissionSets { get; set; }


    public DbSet<BinanceTrade> BinanceTrades { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Interval 1 to * ExchangeState
        modelBuilder.Entity<ExchangeInfo>()
            .HasOne(es => es.UpdateInterval)
            .WithMany(i => i.ExchangeInfos)
            .HasForeignKey(es => es.UpdateIntervalId)
            .OnDelete(DeleteBehavior.Restrict); // Change to Restrict or NoAction

        // Interval 1 to * CryptoCurrency
        modelBuilder.Entity<CryptoCurrency>()
            .HasOne(cc => cc.UpdateInterval)
            .WithMany(i => i.CryptoCurrencies)
            .HasForeignKey(cc => cc.UpdateIntervalId)
            .OnDelete(DeleteBehavior.Restrict);// Change to Restrict or NoAction

        // Interval 1 to * MarketConfiguration
        modelBuilder.Entity<MarketSettings>()
            .HasOne(ms => ms.UpdateInterval)
            .WithMany(i => i.MarketSettingsSet)
            .HasForeignKey(mc => mc.UpdateIntervalId)
            .OnDelete(DeleteBehavior.Restrict); // Change to Restrict or NoAction

        // ExchangeState 1 to * CryptoCurrency
        modelBuilder.Entity<CryptoCurrency>()
            .HasOne(cc => cc.ExchangeInfo)
            .WithMany(es => es.CryptoCurrencies)
            .HasForeignKey(cc => cc.ExchangeInfoId)
            .OnDelete(DeleteBehavior.Restrict); // Change to Restrict or NoAction

        // CurrencySymbol 1 to * CryptoCurrency
        modelBuilder.Entity<CryptoCurrency>()
            .HasOne(cc => cc.CurrencySymbol)
            .WithMany(cs => cs.CryptoCurrency)
            .HasForeignKey(cs => cs.CurrencySymbolId)
            .OnDelete(DeleteBehavior.Restrict); // Change to Restrict or NoAction

        // CryptoCurrency 1 to 1 MarketSettings
        modelBuilder.Entity<MarketSettings>()
            .HasOne(mc => mc.CryptoCurrency)
            .WithOne(cc => cc.MarketSettings)
            .HasForeignKey<MarketSettings>(mc => mc.CryptoCurrencyId);

        // One-to-many relationships

        modelBuilder.Entity<ExchangeFilter>()
            .HasOne(ef => ef.BinanceExchangeInfo)
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
            .HasKey(ot => new { ot.MarketSettingsId, ot.OrderTypeId }); // Composite key

        modelBuilder.Entity<SymbolOrderType>()
            .HasOne(ot => ot.marketSettings)
            .WithMany(s => s.SymbolOrderTypes)
            .HasForeignKey(ot => ot.MarketSettingsId);

        modelBuilder.Entity<SymbolOrderType>()
            .HasOne(ot => ot.OrderType)
            .WithMany(o => o.SymbolOrderTypes)
            .HasForeignKey(ot => ot.OrderTypeId);


        modelBuilder.Entity<PermissionSetPermissions>()
            .HasKey(ps => new { ps.PermissionSetId, ps.PermissionId }); // Composite key

        modelBuilder.Entity<PermissionSetPermissions>()
            .HasOne(ps => ps.PermissionSet)
            .WithMany(s => s.PermissionSetPermissions)
            .HasForeignKey(ps => ps.PermissionSetId);

        modelBuilder.Entity<PermissionSetPermissions>()
            .HasOne(ps => ps.Permission)
            .WithMany(p => p.PermissionSetPermissions)
            .HasForeignKey(ps => ps.PermissionId);
    }
}