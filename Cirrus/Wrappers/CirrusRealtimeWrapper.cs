using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Cirrus.Extensions;
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
        /// <param name="token">Hands a <see cref="JToken"/> from the websocket</param>
        public delegate void OnDataReceivedHandler(object sender, OnDataReceivedEventArgs token);
        
        /// <summary>
        /// The OnDataReceived Event fires when it receives an event from the Ambient Weather API
        /// </summary>
        public event OnDataReceivedHandler OnDataReceived;
        
        /// <summary>
        /// Handler for out OnSubcribe event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="token">Hands a <see cref="JToken"/> from the websocket</param>
        public delegate void OnSubcribeHandler(object sender, OnSubscribeEventArgs token);
        
        /// <summary>
        /// The OnSubcribe event fires when a successful subscription is negotiated with the Ambient Weather Websocket server
        /// </summary>
        public event OnSubcribeHandler OnSubscribe;
        
        /// <summary>
        /// Opens a connection and subscribes to the Ambient Weather service
        /// </summary>
        public Task OpenConnection();
        
        /// <summary>
        /// Unsubscribes from the Ambient Weather Service and closes the SocketIO connection
        /// </summary>
        /// <returns></returns>
        public Task CloseConnection();

        /// <summary>
        /// Subscribes to the Ambient Weather Service
        /// </summary>
        /// <returns></returns>
        public Task Subscribe();
        
        /// <summary>
        /// Unsubscribes from the Ambient Weather Service
        /// This is useful for retrieving a list of invalid API keys, without closing the websocket connection or destroying this instance.
        /// </summary>
        public Task Unsubscribe();
    }

    public sealed class CirrusRealtime : ICirrusRealtime
    {
        private bool _disposed;
        private readonly ILogger<CirrusRealtime> _log;
        
        private SocketIO? Client { get; }
        private Uri BaseAddress { get; } = new("https://dash2.ambientweather.net");
        
        private CirrusConfig Options { get; }
        
        /// <inheritdoc cref="OnDataReceived"/>
        public event ICirrusRealtime.OnDataReceivedHandler OnDataReceived;
        
        /// <inheritdoc cref="OnSubscribe"/>
        public event ICirrusRealtime.OnSubcribeHandler OnSubscribe;
        
        public CirrusRealtime(IOptions<CirrusConfig> options, ILogger<CirrusRealtime>? logger = null)
        {
            _log = logger ?? NullLogger<CirrusRealtime>.Instance;
            
            Options = options.Value;
            
            var applicationKey = Options.ApplicationKey;
            
            Client = new SocketIO(BaseAddress, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    {"api", "1"},
                    {"applicationKey", applicationKey}
                },
                Reconnection = true,
                ReconnectionDelay = 5000, // reconnect after 5 seconds
                ReconnectionDelayMax = 30000
            });
        }

        public async Task OpenConnection()
        {
            var apiKeys = Options.ApiKeys;
            
            Client!.On("subscribed", OnInternalSubscribeEvent);
            Client!.On("data", OnInternalDataEvent);

            Client!.OnConnected += OnInternalConnectEvent;
            Client!.OnDisconnected += OnInternalDisconnectEvent;
            
            _log.LogInformation("Opening websocket connection: {BaseAddress}", BaseAddress.AbsolutePath);
            await Client!.ConnectAsync();
            
            _log.LogInformation("Sending Subcribe Command: {BaseAddress}", BaseAddress.AbsolutePath);
            await Client!.EmitAsync("subscribe", apiKeys);
        }

        public async Task CloseConnection()
        {
            await Client!.EmitAsync("unsubscribe");
            await Client!.EmitAsync("disconnect");
            await Client!.DisconnectAsync();
        }

        public async Task Subscribe()
        {
            if (Client is not null)
            {
                var apiKeys = Options.ApiKeys;
                
                _log.LogInformation("Sending Subcribe Command: {BaseAddress}", BaseAddress.AbsolutePath);
                await Client!.EmitAsync("subscribe", apiKeys);
            }
        }

        public async Task Unsubscribe()
        {
            if (Client is not null)
            {
                _log.LogInformation("Unsubscribing from the ambient weather websocket service");
                await Client.EmitAsync("unsubscribe");
            }
        }

        private void OnInternalDisconnectEvent(object sender, string e)
        {
            _log.LogInformation("API Disconnected");
        }

        private void OnInternalConnectEvent(object sender, EventArgs e)
        {
            _log.LogInformation("Connected to API");
        }

        private async void OnInternalSubscribeEvent(SocketIOResponse obj)
        {
            _log.LogInformation("Subscribed to service");
            
            var userDevice = await obj.GetValue().ToObjectAsync<UserDevice>();
            OnSubscribe.Invoke(this, new OnSubscribeEventArgs(userDevice));
        }

        private async void OnInternalDataEvent(SocketIOResponse obj)
        {
            _log.LogInformation("Received data event");

            var device = await obj.GetValue().ToObjectAsync<Device>();
            OnDataReceived.Invoke(this, new OnDataReceivedEventArgs(device));
        }

        private async void KeepConnectionAlive(object source, ElapsedEventArgs e)
        {
            // This "ping" event emulates a keep-alive message to prevent the API from disconnecting
            _log.LogInformation("Sending ping keep-alive");
            await Client!.EmitAsync("ping");
        }
        
        private async ValueTask ReleaseUnmanagedResourcesAsync()
        {
            await ReleaseSocketIOResources();
            
            // Tell the API we are disconnecting
            await Client!.EmitAsync("disconnect");
            
            // Disconnect
            await Client!.DisconnectAsync();
        }

        private async void ReleaseUnmanagedResources()
        {
            await ReleaseSocketIOResources();
            
            // Tell the API we are disconnecting
            await Client!.EmitAsync("disconnect");
            
            // Disconnect
            await Client!.DisconnectAsync();
        }

        private Task ReleaseSocketIOResources()
        {
            // Unregister our handlers from SocketIO
            Client!.Off("subscribed");
            Client!.Off("data");
            
            // Unregister our internal handlers
            Client!.OnConnected -= OnInternalConnectEvent;
            Client!.OnDisconnected -= OnInternalDisconnectEvent;

            return Task.CompletedTask;
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
    
    public sealed record OnDataReceivedEventArgs(Device Device);
    public sealed record OnSubscribeEventArgs(UserDevice UserDevice);
}