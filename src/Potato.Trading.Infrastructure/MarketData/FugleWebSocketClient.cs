using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Potato.Trading.Core.Entities;
using Potato.Trading.Core.Interfaces;
using MarketDataEntity = Potato.Trading.Core.Entities.MarketData;

namespace Potato.Trading.Infrastructure.MarketData;

public class FugleWebSocketClient : IMarketDataStreamClient
{
    private readonly ClientWebSocket _webSocket;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FugleWebSocketClient> _logger;
    private readonly string _apiKey;

    public FugleWebSocketClient(IConfiguration configuration, ILogger<FugleWebSocketClient> logger)
    {
        _webSocket = new ClientWebSocket();
        _configuration = configuration;
        _logger = logger;
        _apiKey = _configuration["Trading:FugleApiKey"] ?? throw new ArgumentNullException("FugleApiKey not found in configuration");
    }

    public async Task ConnectAsync(string symbol, Func<MarketDataEntity, Task> onMessage, CancellationToken cancellationToken)
    {
        var uri = new Uri($"wss://api.fugle.tw/marketdata/v1.0/stock/streaming?apiToken={_apiKey}");
        
        try
        {
            await _webSocket.ConnectAsync(uri, cancellationToken);
            _logger.LogInformation("Connected to Fugle WebSocket for {Symbol}", symbol);

            // Send subscription message if required by protocol
            // var subscribeMessage = ...
            // await SendAsync(subscribeMessage, cancellationToken);

            var buffer = new byte[4096];
            while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                    _logger.LogInformation("WebSocket closed by server.");
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    // Parse message and invoke callback
                    // var marketData = Parse(message);
                    // await onMessage(marketData);
                    
                    // Placeholder:
                    await onMessage(new MarketDataEntity { Symbol = symbol, Timestamp = DateTime.UtcNow });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebSocket error for {Symbol}", symbol);
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
        }
    }
}
