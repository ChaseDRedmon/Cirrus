using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Adapters;
using Cirrus.Helpers;
using Cirrus.Models;
using Microsoft.Extensions.Options;
using SocketIOClient;

namespace Cirrus.Realtime;

public class CirrusRealtime : ICirrusRealtime
{
    public event EventHandler<OnSubscribeEventArgs>? Subscribed;

    public event EventHandler<OnDataReceivedEventArgs>? DataReceived;
    public event EventHandler Connected;
    public event EventHandler Disconnected;
    
    
    private bool _disposed;
    private SocketIO? Client { get; set; }
    private Uri BaseAddress { get; } = new("https://rt2.ambientweather.net/");
    
    private readonly IOptions<CirrusConfig> _options;
    private readonly ICirrusLoggerAdapter<CirrusRealtime>? _log;
    private readonly List<string>? _apiKeys;
    private readonly string? _applicationKey;

    public CirrusRealtime(IOptions<CirrusConfig> options, ICirrusLoggerAdapter<CirrusRealtime>? logger = null)
    {
        _log = logger;
        _log?.Debug("Creating Realtime Class");

        _applicationKey = options.Value.ApplicationKey;
        _apiKeys = options.Value.ApiKeys;
    }

    /// <inheritdoc />
    public async Task OpenConnection(CancellationToken token = default)
    {
        Client ??= CreateClient();

        _log?.Information("Opening connection to WebSocket Endpoint");
        _log?.Debug("ApiKeys: {@Keys}", _apiKeys);
        _log?.Debug("Subscribing event handlers");

        Client!.On("subscribed", OnInternalSubscribeEvent);
        Client!.On("data", OnInternalDataEvent);
        Client!.OnConnected += OnInternalConnectEvent;
        Client!.OnDisconnected += OnInternalDisconnectEvent;

        _log?.Information("Opening websocket connection: {BaseAddress}", BaseAddress.AbsoluteUri);

        await Client!.ConnectAsync();
        await Client!.EmitAsync("connect", token);
    }

    /// <inheritdoc />
    public async Task CloseConnection(CancellationToken token = default)
    {
        _log?.Information("Closing connection to Ambient Weather WebSocket endpoint");

        await Client!.EmitAsync("unsubscribe", token);
        await Client!.EmitAsync("disconnect", token);
        await Client!.DisconnectAsync();
    }

    /// <inheritdoc />
    public async Task Subscribe(CancellationToken token = default)
    {
        if (Client is null || Client.Disconnected) return;

        _log?.Information("Subscribing to WebSocket service");
        _log?.Information("Sending Subscribe Command: {BaseAddress}", BaseAddress.AbsolutePath);
        _log?.Debug("List: {@List}", _apiKeys);

        await Client!.EmitAsync("subscribe", token, _apiKeys);
    }

    /// <inheritdoc />
    public async Task Unsubscribe(CancellationToken token = default)
    {
        if (Client is null || Client.Disconnected) return;

        _log?.Information("Unsubscribing from the Ambient Weather websocket service");
        await Client.EmitAsync("unsubscribe", token);
    }
    
    public void Dispose()
    {
        Dispose(true);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
    }
    
    // TODO: We need to communicate an error if the API Keys or Application Keys are invalid to the user
    protected virtual void OnInternalSubscribeEvent(SocketIOResponse obj)
    {
        var response = obj.GetValue();

        _log?.Information("Subscribed to service");
        _log?.Trace("Object: {@Obj}", response);
        _log?.Trace("Object: {Obj}", response);

        var userDevice = response.Deserialize<Root>();
        Subscribed?.Invoke(this, new OnSubscribeEventArgs(userDevice));
    }

    protected virtual void OnInternalDataEvent(SocketIOResponse obj)
    {
        var response = obj.GetValue();

        _log?.Debug("Received data event");
        _log?.Trace("Object: {@Obj}", response);
        _log?.Trace("Object: {Obj}", response);

        var device = response.Deserialize<Device>();
        DataReceived?.Invoke(this, new OnDataReceivedEventArgs(device));
    }
    
    /// <summary>
    /// Creates a new SocketIO Client to connect to Ambient Weather's Websocket endpoint.
    /// </summary>
    /// <returns>Returns a new Socket IO Client.</returns>
    /// <exception cref="ArgumentException">Throws an argument exception if <see cref="_applicationKey"/> or <see cref="_apiKeys"/> is null, empty, or whitespace.</exception>
    private SocketIO CreateClient()
    {
        Check.IsNullOrWhitespace(_applicationKey);
        Check.AreAllNullOrWhiteSpace(_apiKeys);

        return new SocketIO(BaseAddress, new SocketIOOptions
        {
            EIO = 4,
            Query = new Dictionary<string, string>
            {
                { "api", "1" },
                { "applicationKey", _applicationKey! }
            },
            Reconnection = true,
            ReconnectionDelay = 5000,
            ReconnectionDelayMax = 30000,
        });
    }
    
    private void OnInternalDisconnectEvent(object sender, string e)
    {
        _log?.Information("API Disconnected");
        Disconnected.Invoke(this, EventArgs.Empty);
    }

    private async void OnInternalConnectEvent(object sender, EventArgs e)
    {
        _log?.Information("Connected to API");
        Connected.Invoke(this, EventArgs.Empty);
        
        _log?.Information("Sending Subscribe Command: {BaseAddress}", BaseAddress.AbsoluteUri);
        _log?.Debug("List: {@list}", _apiKeys);

        await Client!.EmitAsync("subscribe", new ApiKeyWrapper(_apiKeys));
    }

    private async ValueTask ReleaseUnmanagedResourcesAsync()
    {
        ReleaseSocketIOResources();

        // Tell the API we are disconnecting
        await Client!.EmitAsync("disconnect");

        // Disconnect
        await Client!.DisconnectAsync();
    }

    private async void ReleaseUnmanagedResources()
    {
        ReleaseSocketIOResources();

        // Tell the API we are disconnecting
        await Client!.EmitAsync("disconnect");

        // Disconnect
        await Client!.DisconnectAsync();
    }

    private void ReleaseSocketIOResources()
    {
        // Unregister our handlers from SocketIO
        Client!.Off("subscribed");
        Client!.Off("data");

        // Unregister our internal handlers
        Client!.OnConnected -= OnInternalConnectEvent;
        Client!.OnDisconnected -= OnInternalDisconnectEvent;
    }
    
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            // Release managed objects
            if (disposing)
            {
                // Dispose of managed resources
            }

            // Release unmanaged objects
            ReleaseUnmanagedResources();
        }

        _disposed = true;
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed)
        {
            // Release managed objects
            if (disposing)
            {
                // Dispose of managed resources
            }

            // Release unmanaged objects
            await ReleaseUnmanagedResourcesAsync();
        }

        _disposed = true;
    }
}