using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Helpers;
using Cirrus.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SocketIOClient;

namespace Cirrus.Wrappers
{
    public interface ICirrusRealtime : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Handler for our OnDataReceived event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="token">Hands a <see cref="JToken"/> from the websocket.</param>
        delegate void OnDataReceivedHandler(object sender, OnDataReceivedEventArgs token);

        /// <summary>
        /// The OnDataReceived Event fires when it receives an event from the Ambient Weather API
        /// </summary>
        event OnDataReceivedHandler OnDataReceived;

        /// <summary>
        /// Handler for out OnSubcribe event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="token">Hands a <see cref="JToken"/> from the websocket.</param>
        delegate void OnSubcribeHandler(object sender, OnSubscribeEventArgs token);

        /// <summary>
        /// The OnSubcribe event fires when a successful subscription is negotiated with the Ambient Weather Websocket server
        /// </summary>
        event OnSubcribeHandler OnSubscribe;

        /// <summary>
        /// Opens a connection and subscribes to the Ambient Weather service
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Returns a <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentException">Throws an argument exception if <see cref="CirrusConfig.ApplicationKey"/> or <see cref="CirrusConfig.ApiKeys"/> is null, empty, or whitespace.</exception>
        Task OpenConnection(CancellationToken token = default);

        /// <summary>
        /// Unsubscribes from the Ambient Weather Service and closes the SocketIO connection
        /// </summary>
        /// <returns>Returns a <see cref="Task"/>.</returns>
        /// <param name="token">Cancellation token.</param>
        Task CloseConnection(CancellationToken token = default);

        /// <summary>
        /// Subscribes to the Ambient Weather Service
        /// </summary>
        /// <returns>Returns a <see cref="Task"/>.</returns>
        /// <param name="token">Cancellation token.</param>
        Task Subscribe(CancellationToken token = default);

        /// <summary>
        /// Unsubscribes from the Ambient Weather Service
        /// This is useful for retrieving a list of invalid API keys, without closing the websocket connection or destroying this instance.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Returns a <see cref="Task"/>.</returns>
        Task Unsubscribe(CancellationToken token = default);
    }

    public record ApiKeyWrapper(IEnumerable<string> ApiKeys)
    {
        [JsonPropertyName("apiKeys")]
        public IEnumerable<string> ApiKeys { get; init; } = ApiKeys;
    }

    public record OnSubscribeEventArgs(Root UserDevice);
    public record OnDataReceivedEventArgs(Device Device);

    public sealed class CirrusRealtime : ICirrusRealtime
    {
        /// <inheritdoc cref="OnDataReceived"/>
        public event ICirrusRealtime.OnDataReceivedHandler OnDataReceived;

        /// <inheritdoc cref="OnSubscribe"/>
        public event ICirrusRealtime.OnSubcribeHandler OnSubscribe;

        private bool _disposed;
        private readonly ILogger<CirrusRealtime> _log;
        private readonly List<string>? _apiKeys;
        private readonly string? _applicationKey;

        private SocketIO? Client { get; set; }
        private Uri BaseAddress { get; } = new("https://rt2.ambientweather.net/");

        public CirrusRealtime(IOptions<CirrusConfig> options, ILogger<CirrusRealtime>? logger = null)
        {
            _log = logger ?? NullLogger<CirrusRealtime>.Instance;
            _log.LogDebug("Creating Realtime Class");

            _applicationKey = options.Value.ApplicationKey;
            _apiKeys = options.Value.ApiKeys;
        }

        /// <inheritdoc />
        public async Task OpenConnection(CancellationToken token = default)
        {
            Client ??= CreateClient();

            _log.LogInformation("Opening connection to WebSocket Endpoint");
            _log.LogDebug("ApiKeys: {@Keys}", _apiKeys);

            _log.LogDebug("Subscribing event handlers");
            Client!.On("subscribed", OnInternalSubscribeEvent);
            Client!.On("data", OnInternalDataEvent);
            Client!.OnConnected += OnInternalConnectEvent;
            Client!.OnDisconnected += OnInternalDisconnectEvent;

            _log.LogInformation("Opening websocket connection: {BaseAddress}", BaseAddress.AbsoluteUri);
            await Client!.ConnectAsync();
            await Client!.EmitAsync("connect", token);
        }

        /// <inheritdoc />
        public async Task CloseConnection(CancellationToken token = default)
        {
            _log.LogInformation("Closing connection to Ambient Weather WebSocket endpoint");

            await Client!.EmitAsync("unsubscribe", token);
            await Client!.EmitAsync("disconnect", token);
            await Client!.DisconnectAsync();
        }

        /// <inheritdoc />
        public async Task Subscribe(CancellationToken token = default)
        {
            if (Client is null) return;

            _log.LogInformation("Subscribing to WebSocket service");
            _log.LogInformation("Sending Subcribe Command: {BaseAddress}", BaseAddress.AbsolutePath);
            _log.LogDebug("List: {@list}", _apiKeys);

            await Client!.EmitAsync("subscribe", token, _apiKeys);
        }

        /// <inheritdoc />
        public async Task Unsubscribe(CancellationToken token = default)
        {
            if (Client is null) return;

            _log.LogInformation("Unsubscribing from the Ambient Weather websocket service");
            await Client.EmitAsync("unsubscribe", token);
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
                    {"api", "1"},
                    {"applicationKey", _applicationKey}
                },
                Reconnection = true,
                ReconnectionDelay = 5000,
                ReconnectionDelayMax = 30000,
            });
        }

        private void OnInternalDisconnectEvent(object sender, string e)
        {
            _log.LogInformation("API Disconnected");
        }

        private async void OnInternalConnectEvent(object sender, EventArgs e)
        {
            _log.LogInformation("Connected to API");
            _log.LogInformation("Sending Subcribe Command: {BaseAddress}", BaseAddress.AbsoluteUri);
            _log.LogDebug("List: {@list}", _apiKeys);
            
            await Client!.EmitAsync("subscribe", new ApiKeyWrapper(_apiKeys));
        }
        
        // TODO: We need to communicate an error if the API Keys or Application Keys are invalid to the user
        private void OnInternalSubscribeEvent(SocketIOResponse obj)
        {
            _log.LogInformation("Subscribed to service");
            _log.LogTrace("Object: {@Obj}", obj.GetValue());
            _log.LogTrace("Object: {Obj}", obj.GetValue());

            var userDevice = obj.GetValue().Deserialize<Root>();
            OnSubscribe.Invoke(this, new OnSubscribeEventArgs(userDevice));
        }

        private void OnInternalDataEvent(SocketIOResponse obj)
        {
            _log.LogDebug("Received data event");

            _log.LogTrace("Object: {@Obj}", obj.GetValue());
            _log.LogTrace("Object: {Obj}", obj.GetValue());

            var device = obj.GetValue().Deserialize<Device>();
            OnDataReceived.Invoke(this, new OnDataReceivedEventArgs(device));
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
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

        ~CirrusRealtime()
        {
            Dispose(false);
        }
    }
}