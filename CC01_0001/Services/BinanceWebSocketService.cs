using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using CC01_0001.Models;
using CC01_0001.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CC01_0001.Services;

public class BinanceWebSocketHostedService : IHostedService, IDisposable
{
    private ClientWebSocket _webSocket;
    private readonly string _binanceWebSocketUrl = "wss://stream.binance.com:9443/ws/btcusdt@trade";
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BinanceWebSocketHostedService> _logger;
    private readonly ILogger _dbLogger; // New logger for database

    public BinanceWebSocketHostedService(
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory) // Inject ILoggerFactory
    {
        _serviceProvider = serviceProvider;
        _logger = loggerFactory.CreateLogger<BinanceWebSocketHostedService>();
        _dbLogger = loggerFactory.CreateLogger("Database"); // Create a logger with category "Database"
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Binance WebSocket Hosted Service started.");

        Task.Run(async () =>
        {
            await ConnectAndReceive(cancellationToken);
        }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Binance WebSocket Hosted Service stopped.");
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Service stopping", cancellationToken);
        }
    }

    private async Task ConnectAndReceive(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _webSocket = new ClientWebSocket();
            try
            {
                await _webSocket.ConnectAsync(new Uri(_binanceWebSocketUrl), cancellationToken);

                var buffer = new byte[1024 * 4];
                while (!cancellationToken.IsCancellationRequested && _webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await ProcessBinanceData(json, cancellationToken, dbContext);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogInformation("WebSocket connection closed.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket connection or receive error");
            }
        }
    }

    private async Task ProcessBinanceData(string json, CancellationToken cancellationToken, ApplicationDbContext dbContext)
    {
        try
        {
            BinanceTrade trade = JsonSerializer.Deserialize<BinanceTrade>(json);
            if (trade != null)
            {
                dbContext.BinanceTrades.Add(trade);
                await dbContext.SaveChangesAsync(cancellationToken);
                _dbLogger.LogInformation("Trade saved to database."); // Use _dbLogger
            }
        }
        catch (Exception ex)
        {
            _dbLogger.LogError(ex, "Database error"); // Use _dbLogger
        }
    }

    public void Dispose()
    {
        _webSocket?.Dispose();
    }
}