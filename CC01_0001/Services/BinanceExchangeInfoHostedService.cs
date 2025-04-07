/*
    Models/BinanceExchangeInfo.cs
    Version: 0.2.0
    (c) 2024, Minh Tri Tran, with assistance from Google's Gemini - Licensed under CC BY 4.0
    https://creativecommons.org/licenses/by/4.0/
*/
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CC01_0001.Models;
using CC01_0001.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Runtime.InteropServices.Marshalling;



namespace CC01_0001.Services;

public class BinanceExchangeInfoHostedService : IHostedService, IDisposable
{
    private readonly ILogger<BinanceExchangeInfoHostedService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _binanceExchangeInfoUrl = "https://api.binance.com/api/v3/exchangeInfo";
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;
    private JsonSerializerOptions _jsonOptions;


    public BinanceExchangeInfoHostedService(
        ILogger<BinanceExchangeInfoHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _serviceProvider = serviceProvider;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Binance Exchange Info Service is starting.");

        // Set the timer to run immediately and then daily at 3:00 AM UTC
        var now = DateTime.UtcNow;
        var nextRun = new DateTime(now.Year, now.Month, now.Day, 3, 0, 0, DateTimeKind.Utc);
        if (nextRun <= now)
        {
            nextRun = nextRun.AddDays(1);
        }

        var timeToWait = TimeSpan.Zero; // Run immediately
        _timer = new Timer(DoWork, null, timeToWait, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        _logger.LogInformation("Binance Exchange Info Service is working.");

        ApplicationDbContext dbContext;
        HttpResponseMessage httpResponse;

        String jsonString;
        IServiceScope scope;

        try
        {

            httpResponse = await _httpClient.GetAsync(_binanceExchangeInfoUrl);
            httpResponse.EnsureSuccessStatusCode();
            jsonString = await httpResponse.Content.ReadAsStringAsync();

            using (scope = _serviceProvider.CreateScope())
            {
                dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                MakeUpdate(dbContext, DateTime.UtcNow, jsonString);
                dbContext.SaveChanges();
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and saving Binance exchange info.");
        }

        _logger.LogInformation("Binance Exchange Info Service finished working.");
    }



    private void MakeUpdate(ApplicationDbContext dbContext, DateTime lastUpdate, string json)
    {
        UpdateInterval updateInterval;
        byte[] byteBuffer;
        JsonReaderOptions readerOptions;
        Utf8JsonReader reader;


        updateInterval = new UpdateInterval
        {
            Timestamp = lastUpdate
        };
        dbContext.UpdateIntervals.Add(updateInterval);

        byteBuffer = System.Text.Encoding.UTF8.GetBytes(json);
        readerOptions = new JsonReaderOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };
        reader = new Utf8JsonReader(byteBuffer, readerOptions);

        ParseExchangeInfo(dbContext, updateInterval, reader);


        dbContext.SaveChanges();
    }



    /* ParseExchangeInfo
     * -----------------
     * 
     * 
     *
     */
    private void ParseExchangeInfo(ApplicationDbContext dbContext, UpdateInterval updateInterval, Utf8JsonReader reader)
    {

        ExchangeInfo exchangeInfo;
        
        
        exchangeInfo = new ExchangeInfo();
        updateInterval.ExchangeInfos.Add(exchangeInfo);
        dbContext.ExchangeInfos.Add(exchangeInfo);
        dbContext.SaveChanges();


        while (reader.Read())
        {
            var tokenType = reader.TokenType;
            switch (tokenType)
            {
                case JsonTokenType.PropertyName:
                    {
                        string? propertyName = reader.GetString();

                        switch (propertyName)
                        {
                            case "symbols":
                                ParseSymbols(dbContext, exchangeInfo, reader);
                                break;
                            //case "exchangeFilters":
                            //    var exchangeFilters = JsonSerializer.Deserialize<List<ExchangeFilter>>(ref reader, _jsonOptions);
                            //    exchangeInfo.ExchangeFilters = exchangeFilters;
                            //    break;
                            case "rateLimits":
                                ParseRateLimits(dbContext, exchangeInfo, reader);
                                break;
                            case "serverTime":
                                reader.Read();
                                exchangeInfo.ServerTime = reader.GetInt64();
                                dbContext.SaveChanges();
                                break;
                            case "timezone":
                                reader.Read();
                                exchangeInfo.Timezone = reader.GetString();
                                dbContext.SaveChanges();
                                break;
                        }
                    }
                    break;
                default:

                    break;
            }
        }

    }

    private void ParseRateLimits(ApplicationDbContext dbContext, ExchangeInfo exchangeInfo, Utf8JsonReader reader)
    {
        reader.Read(); // Move to the start of the array
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    RateLimit rateLimit = new RateLimit();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            string? propertyName = reader.GetString();
                            reader.Read(); // Move to the property value
                            switch (propertyName)
                            {
                                case "rateLimitType":
                                    rateLimit.RateLimitType = reader.GetString();
                                    break;
                                case "interval":
                                    rateLimit.Interval = reader.GetString();
                                    break;
                                case "intervalNum":
                                    rateLimit.IntervalNum = reader.GetInt32();
                                    break;
                                case "limit":
                                    rateLimit.Limit = reader.GetInt32();
                                    break;
                            }
                        }
                    }

                    // Check if RateLimit already exists in the database
                    var existingRateLimit = dbContext.RateLimits.FirstOrDefault(rl =>
                        rl.RateLimitType == rateLimit.RateLimitType &&
                        rl.Interval == rateLimit.Interval &&
                        rl.IntervalNum == rateLimit.IntervalNum &&
                        rl.Limit == rateLimit.Limit);

                    if (existingRateLimit == null)
                    {
                        dbContext.RateLimits.Add(rateLimit);
                        dbContext.SaveChanges(); // Save the new RateLimit immediately
                        rateLimit = dbContext.RateLimits.FirstOrDefault(rl =>
                            rl.RateLimitType == rateLimit.RateLimitType &&
                            rl.Interval == rateLimit.Interval &&
                            rl.IntervalNum == rateLimit.IntervalNum &&
                            rl.Limit == rateLimit.Limit);
                    }
                    else
                    {
                        rateLimit = existingRateLimit; // Use the existing RateLimit
                    }

                    // Create and add BinanceExchangeRateLimits
                    var binanceExchangeRateLimit = new BinanceExchangeRateLimits
                    {
                        BinanceExchangeId = exchangeInfo.Id,
                        RateLimitId = rateLimit.Id
                    };

                    // Check if the join entry already exists
                    if (!dbContext.BinanceExchangeRateLimits.Any(berl =>
                            berl.BinanceExchangeId == binanceExchangeRateLimit.BinanceExchangeId &&
                            berl.RateLimitId == binanceExchangeRateLimit.RateLimitId))
                    {
                        dbContext.BinanceExchangeRateLimits.Add(binanceExchangeRateLimit);
                    }
                }
            }
            dbContext.SaveChanges(); // Save BinanceExchangeRateLimits
        }
    }


    private void ParseSymbols(ApplicationDbContext dbContext, ExchangeInfo exchangeInfo, Utf8JsonReader reader)
    {

        var setName = reader.GetString();
        ICollection<CryptoCurrency> cryptoCurrencies = exchangeInfo.CryptoCurrencies;


        reader.Read();
        var tokenType = reader.TokenType; 
        if (tokenType == JsonTokenType.StartArray)
        {

            CryptoCurrency cryptoCurrency;
            MarketSettings marketSettings;


            while (reader.Read())
            {
                // Parse Symbol
                cryptoCurrency = new CryptoCurrency();
                marketSettings = cryptoCurrency.MarketSettings;

                cryptoCurrencies.Add(cryptoCurrency);


                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    // Parse Properties
                    while (reader.Read())
                    {
                        var tokenType2 = reader.TokenType;
                        if (tokenType2 == JsonTokenType.PropertyName)
                        {
                            string? propertyName = reader.GetString();

                            switch (propertyName)
                            {
                                case "permissionSets":
                                    reader.Read();
                                    ParseMarketPermissionSets(dbContext, marketSettings, reader);
                                    break;
                                case "filters":
                                    var filters = JsonSerializer.Deserialize<List<Filter>>(ref reader, _jsonOptions);
                                    marketSettings.Filters = filters;
                                    break;
                                case "isMarginTradingAllowed":
                                    reader.Read();
                                    marketSettings.IsMarginTradingAllowed = reader.GetBoolean();
                                    break;
                                case "isSpotTradingAllowed":
                                    reader.Read();
                                    marketSettings.IsSpotTradingAllowed = reader.GetBoolean();
                                    break;
                                case "cancelReplaceAllowed":
                                    reader.Read();
                                    marketSettings.CancelReplaceAllowed = reader.GetBoolean();
                                    break;
                                case "allowTrailingStop":
                                    reader.Read();
                                    marketSettings.AllowTrailingStop = reader.GetBoolean();
                                    break;
                                case "quoteOrderQtyMarketAllowed":
                                    reader.Read();
                                    marketSettings.QuoteOrderQtyMarketAllowed = reader.GetBoolean();
                                    break;
                                case "otoAllowed":
                                    reader.Read();
                                    marketSettings.OtoAllowed = reader.GetBoolean();
                                    break;
                                case "ocoAllowed":
                                    reader.Read();
                                    marketSettings.OcoAllowed = reader.GetBoolean();
                                    break;
                                case "icebergAllowed":
                                    reader.Read();
                                    marketSettings.IcebergAllowed = reader.GetBoolean();
                                    break;
                                case "orderTypes":
                                    ParseMarketOrderTypes(dbContext, marketSettings, reader);
                                    break;
                                case "quoteCommissionPrecision":
                                    reader.Read();
                                    marketSettings.QuoteCommissionPrecision = reader.GetInt32();
                                    break;
                                case "baseCommissionPrecision":
                                    reader.Read();
                                    marketSettings.BaseCommissionPrecision = reader.GetInt32();
                                    break;
                                case "quoteAssetPrecision":
                                    reader.Read();
                                    marketSettings.QuoteAssetPrecision = reader.GetInt32();
                                    break;
                                case "quotePrecision":
                                    reader.Read();
                                    marketSettings.QuotePrecision = reader.GetInt32();
                                    break;
                                case "quoteAsset":
                                    reader.Read();
                                    marketSettings.QuoteAsset = reader.GetString();
                                    break;
                                case "baseAssetPrecision":
                                    reader.Read();
                                    marketSettings.BaseAssetPrecision = reader.GetInt32();
                                    break;
                                case "baseAsset":
                                    reader.Read();
                                    marketSettings.BaseAsset = reader.GetString();
                                    break;
                                case "status":
                                    reader.Read();
                                    marketSettings.Status = reader.GetString();
                                    break;
                                case "symbol":
                                    reader.Read();
                                    String? symbol = reader.GetString();
                                    if (symbol != null)
                                    {
                                        // lookup if symbol exists.
                                        CurrencySymbol? symbol2 = dbContext.CurrencySymbols.FirstOrDefault(cs => cs.Symbol == symbol);
                                        if (symbol2 == null)
                                        {
                                            symbol2 = new CurrencySymbol { Symbol = symbol };
                                            dbContext.CurrencySymbols.Add(symbol2);
                                        }
                                        cryptoCurrency.CurrencySymbol = symbol2;
                                        dbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        // Handle the case where the symbol is null
                                        _logger.LogWarning("Symbol is null.");
                                    }
                                    break;
                            }
                        }
                        else if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            dbContext.SaveChanges();
                            break;
                        }
                    }


                }
                else if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }
            }
        }
        else
        {
            // Handle the case where the expected token type is not found
        }

        reader.Read();
        if (reader.TokenType == JsonTokenType.EndArray)
        {
            // End of symbols array
            dbContext.SaveChanges();
        }
        else
        {
            // Handle the case where the expected token type is not found
        }

    }


    private void ParseMarketOrderTypes(DbContext dbContext, MarketSettings marketSettings, Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string orderTypeTag = reader.GetString();

                    // Find or create the OrderType
                    OrderType orderType = new OrderType 
                    { 
                        TypeTag = orderTypeTag
                    };

                    // Create the join table entry
                    SymbolOrderType symbolOrderType = new SymbolOrderType
                    {
                        marketSettings = marketSettings,
                        OrderType = orderType
                    };

                    // Add the join entry to the Symbol
                    if (marketSettings.SymbolOrderTypes == null)
                    {
                        marketSettings.SymbolOrderTypes = new List<SymbolOrderType>();
                    }
                    marketSettings.SymbolOrderTypes.Add(symbolOrderType);
                }
                else if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }
            }
        }
    }


    private void ParseMarketPermissionSets(ApplicationDbContext dbContext, MarketSettings marketSettings, Utf8JsonReader reader)
    {
        JsonTokenType tokenType;
        List<PermissionSet> permissionSets;
        PermissionSet permissionSet;


        tokenType = reader.TokenType;
        while (reader.Read())
        {

            tokenType = reader.TokenType;
            if (tokenType == JsonTokenType.StartArray)
            {
                permissionSets = new List<PermissionSet>();
                ParsePermissionSet(dbContext, permissionSets, reader);

            }
            else if (tokenType == JsonTokenType.EndArray)
            {
                // End of permissionSets array
                dbContext.SaveChanges();
                break;
            }
        }
    }

    private void ParsePermissionSet(ApplicationDbContext dbContext, List<PermissionSet> permissionSets, Utf8JsonReader reader)
    {
        JsonTokenType tokenType;
        PermissionSet permissionSet = null; // Initialize to null


        while (true)
        {
            tokenType = reader.TokenType;
            if (tokenType == JsonTokenType.StartArray)
            {
                permissionSet = new PermissionSet();
                permissionSet.PermissionSetPermissions = new List<PermissionSetPermissions>();
            }
            else if (tokenType == JsonTokenType.String)
            {

                var permissionTag = reader.GetString(); // Corrected variable name

                Permission permission = new Permission { PermissionTag = permissionTag };

                // Create the join table entry
                PermissionSetPermissions permissionSetPermission = new PermissionSetPermissions
                {
                    PermissionSet = permissionSet,
                    Permission = permission
                };

                permissionSet.PermissionSetPermissions.Add(permissionSetPermission); // Add to the PermissionSet
            }
            else if (tokenType == JsonTokenType.EndArray)
            {
                if (permissionSet != null) // Check if a PermissionSet was created
                {
                    permissionSets.Add(permissionSet); // Add the PermissionSet to the list
                }
                break;
            }

            reader.Read();
        }

    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Binance WebSocket Hosted Service stopped.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }


    public void Dispose()
    {
        _timer?.Dispose();
        _httpClient?.Dispose();
    }
}