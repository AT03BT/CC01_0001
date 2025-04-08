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
        ExchangeUpdate updateInterval;
        byte[] byteBuffer;
        JsonReaderOptions readerOptions;
        Utf8JsonReader reader;


        updateInterval = new ExchangeUpdate
        {
            Timestamp = lastUpdate
        };
        dbContext.ExchangeUpdates.Add(updateInterval);

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



    private void ParseExchangeInfo(ApplicationDbContext dbContext, ExchangeUpdate exchangeUpdate, Utf8JsonReader reader)
    {
        ExchangeInfo exchangeInfo = new ExchangeInfo();
        exchangeUpdate.ExchangeInfo = exchangeInfo;
        dbContext.ExchangeInfos.Add(exchangeInfo);
        dbContext.SaveChanges(); // Save ExchangeInfo and UpdateInterval

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
                                ParseSymbols(dbContext, exchangeInfo, reader); // Pass updateInterval
                                break;
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
                        break;
                    }
                default:
                    break;
            }
        }
    }

    private void ParseSymbols(ApplicationDbContext dbContext, ExchangeInfo exchangeInfo, Utf8JsonReader reader) // Added updateInterval
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {

                    MarketSetup marketSetup = new MarketSetup()
                    {
                        ExchangeInfo = exchangeInfo,
                    };

                    dbContext.MarketSetups.Add(marketSetup);

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            string? propertyName = reader.GetString();

                            switch (propertyName)
                            {
                                case "permissionSets":
                                    reader.Read();
                                    dbContext.SaveChanges();
                                    ParsePermissionSetSpacer(dbContext, marketSetup, reader); // Changed marketSettings to cryptoCurrency
                                    break;
                                case "filters":
                                    var filters = JsonSerializer.Deserialize<List<Filter>>(ref reader, _jsonOptions);
                                    marketSetup.Filters = filters;
                                    break;
                                case "isMarginTradingAllowed":
                                    reader.Read();
                                    marketSetup.IsMarginTradingAllowed = reader.GetBoolean();
                                    break;
                                case "isSpotTradingAllowed":
                                    reader.Read();
                                    marketSetup.IsSpotTradingAllowed = reader.GetBoolean();
                                    break;
                                case "cancelReplaceAllowed":
                                    reader.Read();
                                    marketSetup.CancelReplaceAllowed = reader.GetBoolean();
                                    break;
                                case "allowTrailingStop":
                                    reader.Read();
                                    marketSetup.AllowTrailingStop = reader.GetBoolean();
                                    break;
                                case "quoteOrderQtyMarketAllowed":
                                    reader.Read();
                                    marketSetup.QuoteOrderQtyMarketAllowed = reader.GetBoolean();
                                    break;
                                case "otoAllowed":
                                    reader.Read();
                                    marketSetup.OtoAllowed = reader.GetBoolean();
                                    break;
                                case "ocoAllowed":
                                    reader.Read();
                                    marketSetup.OcoAllowed = reader.GetBoolean();
                                    break;
                                case "icebergAllowed":
                                    reader.Read();
                                    marketSetup.IcebergAllowed = reader.GetBoolean();
                                    break;
                                case "orderTypes":
                                    reader.Read();
                                    ParseMarketOrderTypes(dbContext, marketSetup, reader);
                                    break;
                                case "quoteCommissionPrecision":
                                    reader.Read();
                                    marketSetup.QuoteCommissionPrecision = reader.GetInt32();
                                    break;
                                case "baseCommissionPrecision":
                                    reader.Read();
                                    marketSetup.BaseCommissionPrecision = reader.GetInt32();
                                    break;
                                case "quoteAssetPrecision":
                                    reader.Read();
                                    marketSetup.QuoteAssetPrecision = reader.GetInt32();
                                    break;
                                case "quotePrecision":
                                    reader.Read();
                                    marketSetup.QuotePrecision = reader.GetInt32();
                                    break;
                                case "quoteAsset":
                                    reader.Read();
                                    marketSetup.QuoteAsset = reader.GetString();
                                    break;
                                case "baseAssetPrecision":
                                    reader.Read();
                                    marketSetup.BaseAssetPrecision = reader.GetInt32();
                                    break;
                                case "baseAsset":
                                    reader.Read();
                                    marketSetup.BaseAsset = reader.GetString();
                                    break;
                                case "status":
                                    reader.Read();
                                    marketSetup.Status = reader.GetString();
                                    break;
                                case "symbol":
                                    reader.Read();
                                    marketSetup.Symbol = reader.GetString();
                                    break;
                            }
                        }
                        else if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            dbContext.SaveChanges(); // Save each CryptoCurrency
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
        dbContext.SaveChanges(); // Save any remaining changes
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


    private void ParseMarketOrderTypes(DbContext dbContext, MarketSetup marketSetup, Utf8JsonReader reader)
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
                        marketSetup = marketSetup,
                        OrderType = orderType
                    };

                    // Add the join entry to the Symbol
                    if (marketSetup.SymbolOrderTypes == null)
                    {
                        marketSetup.SymbolOrderTypes = new List<SymbolOrderType>();
                    }
                    marketSetup.SymbolOrderTypes.Add(symbolOrderType);
                }
                else if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }
            }
        }
    }


    private void ParsePermissionSetSpacer(ApplicationDbContext dbContext, MarketSetup marketSetup, Utf8JsonReader reader)
    {
        JsonTokenType tokenType;
        PermissionSetSpacer permissionSetSpacer = new PermissionSetSpacer();

        tokenType = reader.TokenType;
        while (reader.Read())
        {

            tokenType = reader.TokenType;
            if (tokenType == JsonTokenType.StartArray)
            {
                marketSetup.PermissionSetSpacer = permissionSetSpacer;
                dbContext.PermissionSetSpacers.Add(permissionSetSpacer);
                ParsePermissionSet(dbContext, permissionSetSpacer, reader);

                dbContext.SaveChanges();
                break;
            }
            else if(tokenType == JsonTokenType.String)
            {
                var tokenStringValue = reader.GetString();
            }
            else if (tokenType == JsonTokenType.EndArray)
            {
                // End of permissionSets array
                dbContext.SaveChanges();
                break;
            }
        }
    }

    private void ParsePermissionSet(ApplicationDbContext dbContext, PermissionSetSpacer permissionSetSpacer, Utf8JsonReader reader)
    {
        JsonTokenType tokenType;
        PermissionSet permissionSet = new PermissionSet();

        tokenType = reader.TokenType;
        while (reader.Read())
        {
            tokenType = reader.TokenType;
            if (tokenType == JsonTokenType.StartArray)
            {
                dbContext.PermissionSets.Add(permissionSet);
                permissionSetSpacer.PermissionSets.Add(permissionSet);
            }
            else if (reader.TokenType == JsonTokenType.String)
            {

                var permissionTag = reader.GetString();

                Permission permission = new Permission { PermissionTag = permissionTag };
                permissionSet.Permissions.Add(permission);
            }
            else if (tokenType == JsonTokenType.EndArray)
            {
                // End of permissionSet array
                dbContext.SaveChanges();
                break;
            }

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