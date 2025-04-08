/*
    Models/BinanceExchangeInfo.cs
    Version: 0.1.0
    (c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
    https://creativecommons.org/licenses/by/4.0/
*/
using Microsoft.CodeAnalysis.Elfie.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CC01_0001.Models;

public class ExchangeUpdate
{
    [Key]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }

    public ExchangeInfo ExchangeInfo { get; set; }
}

public class ExchangeInfo
{
    [Key]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Timezone { get; set; }
    public long? ServerTime { get; set; }

    public int ExchangeUpdateId { get; set; }
    public ExchangeUpdate ExchangeUpdate { get; set; }

    public List<BinanceExchangeRateLimits>? BinanceExchangeRateLimits { get; set; }
    public List<ExchangeFilter>? ExchangeFilters { get; set; }
    public ICollection<MarketSetup> MarketSetups { get; set; }
}

public class MarketSetupPermissionSets
{
    [Key]
    public int Id { get; set; }
}

public class MarketSetup
{
    [Key]
    public int Id { get; set; }

    public string? Symbol { get; set; }
    public string? Status { get; set; }
    public string? BaseAsset { get; set; }
    public int? BaseAssetPrecision { get; set; }
    public string? QuoteAsset { get; set; }
    public int? QuotePrecision { get; set; }
    public int? QuoteAssetPrecision { get; set; }
    public int? BaseCommissionPrecision { get; set; }
    public int? QuoteCommissionPrecision { get; set; }
    public bool? IcebergAllowed { get; set; }
    public bool? OcoAllowed { get; set; }
    public bool? OtoAllowed { get; set; }
    public bool? QuoteOrderQtyMarketAllowed { get; set; }
    public bool? AllowTrailingStop { get; set; }
    public bool? CancelReplaceAllowed { get; set; }
    public bool? IsSpotTradingAllowed { get; set; }
    public bool? IsMarginTradingAllowed { get; set; }

    public int ExchangeInfoId { get; set; } // Foreign key
    public ExchangeInfo ExchangeInfo { get; set; }

    public List<SymbolOrderType>? SymbolOrderTypes { get; set; } = new List<SymbolOrderType>();
    public List<Filter>? Filters { get; set; } = new List<Filter>();
    public PermissionSetSpacer PermissionSetSpacer { get; set; }
}

public class RateLimit
{
    [Key]
    public int Id { get; set; }
    public string RateLimitType { get; set; }
    public string Interval { get; set; }
    public int IntervalNum { get; set; }
    public int Limit { get; set; }

    public List<BinanceExchangeRateLimits>? BinanceExchangeRateLimits { get; set; } = new List<BinanceExchangeRateLimits>();
}

public class BinanceExchangeRateLimits
{
    [Key]
    public int RateLimitId { get; set; }
    public RateLimit? RateLimit { get; set; }

    [Key]
    public int BinanceExchangeId { get; set; }
    public ExchangeInfo? BinanceExchange { get; set; }
}

public class OrderType
{
    [Key]
    public int Id { get; set; }
    public string TypeTag { get; set; }

    public List<SymbolOrderType>? SymbolOrderTypes { get; set; } = new List<SymbolOrderType>();
}

public class SymbolOrderType
{
    [Key]
    public int MarketSetupId { get; set; }
    public MarketSetup marketSetup { get; set; }

    [Key]
    public int OrderTypeId { get; set; }
    public OrderType OrderType { get; set; }
}

public class Filter
{
    [Key]
    public int Id { get; set; }
    public string? FilterType { get; set; }
    public string? MinPrice { get; set; }
    public string? MaxPrice { get; set; }
    public string? TickSize { get; set; }
    public string? MinQty { get; set; }
    public string? MaxQty { get; set; }
    public string? StepSize { get; set; }

    public int? SymbolId { get; set; }
    public MarketSetup? Symbol { get; set; }
}

public class PermissionSetSpacer
{
    [Key]
    public int Id { get; set; }
    public int MarketSetupId { get; set; } // Foreign key

    public MarketSetup MarketSetup { get; set; } // Navigation property
    public ICollection<PermissionSet> PermissionSets { get; set; } = new List<PermissionSet>(); // Updated
}

public class PermissionSet
{
    [Key]
    public int Id { get; set; }

    public int PermissionSetSpacerId { get; set; }
    public PermissionSetSpacer PermissionSetSpacer { get; set; } 

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>(); 
}

public class Permission
{
    [Key]
    public int Id { get; set; }
    public int PermissionSetId { get; set; }

    public PermissionSet PermissionSet { get; set; }
    public string PermissionTag { get; set; }
    public string? Description { get; set; }

}

public class ExchangeFilter
{
    [Key]
    public int Id { get; set; }
    // Add properties for exchange filters if needed
    public int BinanceExchangeInfoId { get; set; } // Foreign key
    public ExchangeInfo ExchangeInfo { get; set; }
}