/*
    Models/BinanceExchangeInfo.cs
    Version: 1.0.0
    (c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
    https://creativecommons.org/licenses/by/4.0/
*/
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CC01_0001.Models;

public class ExchangeHistory
{
    [Key]
    public int Id { get; set; } 
    public DateTime Timestamp { get; set; } 


    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("serverTime")]
    public long? ServerTime { get; set; }

    [JsonPropertyName("symbols")]
    public List<Symbol>? Symbols { get; set; }

    [JsonPropertyName("rateLimits")]
    public List<RateLimit>? RateLimits { get; set; }

    [JsonPropertyName("exchangeFilters")]
    public List<ExchangeFilter>? ExchangeFilters { get; set; }
}

public class RateLimit
{
    [Key]
    public int Id { get; set; }
    public string RateLimitType { get; set; }
    public string Interval { get; set; }
    public int IntervalNum { get; set; }
    public int Limit { get; set; }
    public int BinanceExchangeInfoId { get; set; } // Foreign key
    public ExchangeHistory BinanceExchangeInfo { get; set; }
}

public class Symbol
{
    [Key]
    public int Id { get; set; }
    public string SymbolName { get; set; } // Or use an int Id and make Symbol unique
    public string Status { get; set; }
    public string BaseAsset { get; set; }
    public int BaseAssetPrecision { get; set; }
    public string QuoteAsset { get; set; }
    public int QuotePrecision { get; set; }
    public int QuoteAssetPrecision { get; set; }
    public int BaseCommissionPrecision { get; set; }
    public int QuoteCommissionPrecision { get; set; }
    public bool IcebergAllowed { get; set; }
    public bool OcoAllowed { get; set; }
    public bool OtoAllowed { get; set; }
    public bool QuoteOrderQtyMarketAllowed { get; set; }
    public bool AllowTrailingStop { get; set; }
    public bool CancelReplaceAllowed { get; set; }
    public bool IsSpotTradingAllowed { get; set; }
    public bool IsMarginTradingAllowed { get; set; }
    public int BinanceExchangeInfoId { get; set; } // Foreign key
    public ExchangeHistory? BinanceExchangeInfo { get; set; }

    public List<SymbolOrderType>? SymbolOrderTypes { get; set; }
    public List<Filter>? Filters { get; set; }
    public List<PermissionSet>? PermissionSets { get; set; } // Assuming this is a list of permission sets   
}

public class OrderType
{
    [Key]
    public int Id { get; set; }
    public string TypeTag { get; set; }

    public List<SymbolOrderType>? SymbolOrderTypes { get; set; }
}

public class SymbolOrderType
{
    [Key]
    public int SymbolId { get; set; }
    public Symbol Symbol { get; set; }

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
    public Symbol? Symbol { get; set; }
}

public class PermissionSet
{
    [Key]
    public int Id { get; set; }
    public List<PermissionSetPermissions> PermissionSetPermissions { get; set; }
}

public class Permission
{
    [Key]
    public int Id { get; set; }
    public string PermissionTag { get; set; }
    public string? Description { get; set; }

    public List<PermissionSetPermissions> PermissionSetPermissions { get; set; }
}

public class PermissionSetPermissions
{
    [Key]
    public int PermissionSetId { get; set; } // Foreign key
    public PermissionSet PermissionSet { get; set; }

    [Key]
    public int PermissionId { get; set; } // Foreign key
    public Permission Permission { get; set; }
}

public class ExchangeFilter
{
    [Key]
    public int Id { get; set; }
    // Add properties for exchange filters if needed
    public int BinanceExchangeInfoId { get; set; } // Foreign key
    public ExchangeHistory BinanceExchangeInfo { get; set; }
}