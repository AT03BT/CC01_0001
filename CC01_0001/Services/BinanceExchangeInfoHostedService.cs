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
        BinanceExchangeInfo exchangeInfo = new BinanceExchangeInfo();
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
                ParseExchangeInfo(dbContext, exchangeInfo, jsonString);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and saving Binance exchange info.");
        }

        _logger.LogInformation("Binance Exchange Info Service finished working.");
    }




    private void ParseExchangeInfo(ApplicationDbContext dbContext, BinanceExchangeInfo exchangeInfo, string json)
    {
        exchangeInfo.Timestamp = DateTime.UtcNow;
        var byteBuffer = System.Text.Encoding.UTF8.GetBytes(json);
        dbContext.SaveChanges();

        var options = new JsonReaderOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };
        var reader = new Utf8JsonReader(byteBuffer, options);

        while (reader.Read())
        {
            Console.Write(reader.TokenType);
            var tokenType = reader.TokenType;
            switch (tokenType)
            {
                case JsonTokenType.PropertyName:
                    {
                        string? propertyName = reader.GetString();
                        Console.Write(" ");
                        Console.Write(propertyName);

                        switch (propertyName)
                        {
                            case "symbols":
                                ParseSymbols(dbContext, exchangeInfo, reader, json);
                                break;
                            case "exchangeFilters":
                                var exchangeFilters = JsonSerializer.Deserialize<List<ExchangeFilter>>(ref reader, _jsonOptions);
                                exchangeInfo.ExchangeFilters = exchangeFilters;
                                break;
                            //case "rateLimits":
                            //    var rateLimits = JsonSerializer.Deserialize<List<RateLimit>>(ref reader, _jsonOptions);
                            //    exchangeInfo.RateLimits = rateLimits;
                            //    break;
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


                    //string? text = reader.GetString();
                    //Console.Write("UNEXPECTED VALUE: ");
                    //Console.Write(text);
                    break;
            }
            Console.WriteLine();
        }

    }

    private void ParseSymbols(ApplicationDbContext dbContext, BinanceExchangeInfo exchangeInfo, Utf8JsonReader reader, string json)
    {
        var setName = reader.GetString();
        List<Symbol> symbols = new List<Symbol>();
        exchangeInfo.Symbols = symbols;


        reader.Read();
        var tokenType = reader.TokenType; 
        if (tokenType == JsonTokenType.StartArray)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    Symbol symbol = new Symbol();
                    while (reader.Read())
                    {
                        var tokenType2 = reader.TokenType;
                        if (tokenType2 == JsonTokenType.PropertyName)
                        {
                            string? propertyName = reader.GetString();
                            Console.Write(" ");
                            Console.Write(propertyName);

                            switch (propertyName)
                            {
                                case "permissionSets":
                                    reader.Read();
                                    ParsePermissionSets(dbContext, symbol, reader, json);
                                    break;
                                case "filters":
                                    var filters = JsonSerializer.Deserialize<List<Filter>>(ref reader, _jsonOptions);
                                    symbol.Filters = filters;
                                    break;
                                case "isMarginTradingAllowed":
                                    reader.Read();
                                    symbol.IsMarginTradingAllowed = reader.GetBoolean();
                                    break;
                                case "isSpotTradingAllowed":
                                    reader.Read();
                                    symbol.IsSpotTradingAllowed = reader.GetBoolean();
                                    break;
                                case "cancelReplaceAllowed":
                                    reader.Read();
                                    symbol.CancelReplaceAllowed = reader.GetBoolean();
                                    break;
                                case "allowTrailingStop":
                                    reader.Read();
                                    symbol.AllowTrailingStop = reader.GetBoolean();
                                    break;
                                case "quoteOrderQtyMarketAllowed":
                                    reader.Read();
                                    symbol.QuoteOrderQtyMarketAllowed = reader.GetBoolean();
                                    break;
                                case "otoAllowed":
                                    reader.Read();
                                    symbol.OtoAllowed = reader.GetBoolean();
                                    break;
                                case "ocoAllowed":
                                    reader.Read();
                                    symbol.OcoAllowed = reader.GetBoolean();
                                    break;
                                case "icebergAllowed":
                                    reader.Read();
                                    symbol.IcebergAllowed = reader.GetBoolean();
                                    break;
                                case "orderTypes":
                                    ParseOrderTypes(symbol, reader, json);
                                    break;
                                case "quoteCommissionPrecision":
                                    reader.Read();
                                    symbol.QuoteCommissionPrecision = reader.GetInt32();
                                    break;
                                case "baseCommissionPrecision":
                                    reader.Read();
                                    symbol.BaseCommissionPrecision = reader.GetInt32();
                                    break;
                                case "quoteAssetPrecision":
                                    reader.Read();
                                    symbol.QuoteAssetPrecision = reader.GetInt32();
                                    break;
                                case "quotePrecision":
                                    reader.Read();
                                    symbol.QuotePrecision = reader.GetInt32();
                                    break;
                                case "quoteAsset":
                                    reader.Read();
                                    symbol.QuoteAsset = reader.GetString();
                                    break;
                                case "baseAssetPrecision":
                                    reader.Read();
                                    symbol.BaseAssetPrecision = reader.GetInt32();
                                    break;
                                case "baseAsset":
                                    reader.Read();
                                    symbol.BaseAsset = reader.GetString();
                                    break;
                                case "status":
                                    reader.Read();
                                    symbol.Status = reader.GetString();
                                    break;
                                case "symbol":
                                    reader.Read();
                                    symbol.SymbolName = reader.GetString();
                                    break;
                            }
                        }
                        else if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            symbols.Add(symbol);
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

        //exchangeInfo.Symbols = symbols;
    }

    private void ParseOrderTypes(Symbol symbol, Utf8JsonReader reader, string json)
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
                    OrderType orderType = new OrderType { TypeTag = orderTypeTag };

                    // Create the join table entry
                    SymbolOrderType symbolOrderType = new SymbolOrderType
                    {
                        Symbol = symbol,
                        OrderType = orderType
                    };

                    // Add the join entry to the Symbol
                    if (symbol.SymbolOrderTypes == null)
                    {
                        symbol.SymbolOrderTypes = new List<SymbolOrderType>();
                    }
                    symbol.SymbolOrderTypes.Add(symbolOrderType);
                }
                else if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }
            }
        }
    }

    private void ParsePermissionSets(ApplicationDbContext dbContext, Symbol symbol, Utf8JsonReader reader, string json)
    {
        JsonTokenType tokenType;
        List<PermissionSet> permissionSets;
        PermissionSet permissionSet;

        var symbolName = symbol.SymbolName;
        tokenType = reader.TokenType;
        while (reader.Read())
        {

            tokenType = reader.TokenType;
            if (tokenType == JsonTokenType.StartArray)
            {
                permissionSets = new List<PermissionSet>();
                ParsePermissionSet(dbContext, permissionSets, reader, json);

            }
            else if (tokenType == JsonTokenType.EndArray)
            {
                // End of permissionSets array
                dbContext.SaveChanges();
                break;
            }
        }
    }

    private void ParsePermissionSet(ApplicationDbContext dbContext, List<PermissionSet> permissionSets, Utf8JsonReader reader, string json)
    {
        JsonTokenType tokenType;
        PermissionSet permissionSet = null; // Initialize to null
        int count = 0;

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