﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Cirrus.Extensions;
using Cirrus.Models;
using Microsoft.Extensions.Options;
using Serilog;
using SocketIOClient;

namespace Cirrus.Wrappers
{
    public interface ICirrusRealtime
    {
        /// <summary>
        /// Handler for our OnDataReceived event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="token">Hands a <see cref="JToken"/>> from the websocket</param>
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
        /// Unsubscribes from the ambient weather service
        /// This is useful for retrieving a list of invalid API keys, without closing the websocket connection or destroying this instance.
        /// </summary>
        public Task Unsubscribe();
    }

    public sealed class CirrusRealtime : ICirrusRealtime, IDisposable, IAsyncDisposable
    {
        private bool _disposed;
        private readonly ILogger? _log;
        
        private SocketIO? Client { get; }
        private Uri BaseAddress { get; } = new("https://dash2.ambientweather.net");
        private Timer? Timer { get; }
        
        private CirrusConfig Options { get; }
        
        /// <inheritdoc cref="OnDataReceived"/>
        public event ICirrusRealtime.OnDataReceivedHandler OnDataReceived;
        
        /// <inheritdoc cref="OnSubscribe"/>
        public event ICirrusRealtime.OnSubcribeHandler OnSubscribe;

        public CirrusRealtime(IOptions<CirrusConfig> options, ILogger? logger = null): this(options)
        {
            _log = logger?.ForContext<CirrusRealtime>();
        }
        
        public CirrusRealtime(IOptions<CirrusConfig> options)
        {
            Options = options.Value;
            
            var applicationKey = Options.ApplicationKey;
            
            Client = new SocketIO(BaseAddress, new SocketIOOptions
            {
                EIO = 4,
                Query = new Dictionary<string, string>
                {
                    {"api", "1"},
                    {"applicationKey", applicationKey}
                },
                Reconnection = true,
                ReconnectionDelay = 5000, // reconnect after 5 seconds
                ReconnectionDelayMax = 30000
            });
            
            Timer = new Timer { Interval = 10000 };
            Timer.Elapsed += KeepConnectionAlive;
        }

        public async Task OpenConnection()
        {
            var apiKeys = Options.ApiKeys;
            
            Client!.On("subscribed", OnInternalSubscribeEvent);
            Client!.On("data", OnInternalDataEvent);

            Client!.OnConnected += OnInternalConnectEvent;
            Client!.OnDisconnected += OnInternalDisconnectEvent;
            
            _log?.Information("Opening websocket connection: {BaseAddress}", BaseAddress.AbsolutePath);
            await Client!.ConnectAsync();
            
            _log?.Information("Sending Subcribe Command: {BaseAddress}", BaseAddress.AbsolutePath);
            await Client!.EmitAsync("subscribe", apiKeys);
            
            Timer!.Start();

            await Task.Delay(-1);
        }

        public async Task Unsubscribe()
        {
            if (Client is not null)
            {
                _log?.Information("Unsubscribing from the ambient weather websocket service");
                await Client.EmitAsync("unsubscribe");
            }
        }

        private void OnInternalDisconnectEvent(object sender, string e)
        {
            _log?.Information("API Disconnected");
            Timer!.Stop();
        }

        private void OnInternalConnectEvent(object sender, EventArgs e)
        {
            _log?.Information("Connected to API");

            if (!Timer!.Enabled)
            {
                Timer.Start();
            }
        }

        private async void OnInternalSubscribeEvent(SocketIOResponse obj)
        {
            _log?.Information("Subscribed to service");
            
            var x = await obj.GetValue().ToObjectAsync<UserDevice>();
            OnSubscribe.Invoke(this, new OnSubscribeEventArgs(x));
        }

        private async void OnInternalDataEvent(SocketIOResponse obj)
        {
            _log?.Information("Received data event");

            var x = await obj.GetValue().ToObjectAsync<Device>();
            OnDataReceived.Invoke(this, new OnDataReceivedEventArgs(x));
        }

        private async void KeepConnectionAlive(object source, ElapsedEventArgs e)
        {
            // This "ping" event emulates a keep-alive message to prevent the API from disconnecting
            _log?.Information("Sending ping keep-alive");
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

        private void ReleaseUnmanagedResources()
        {
            ReleaseSocketIOResources().Wait();
            
            // Tell the API we are disconnecting
            Client!.EmitAsync("disconnect").Wait();
            
            // Disconnect
            Client!.DisconnectAsync().Wait();
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
                    Timer?.Close();
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
                    Timer?.Close();
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
    
    public sealed class OnDataReceivedEventArgs
    {
        public OnDataReceivedEventArgs(Device device)
        {
            Device = device;
        }
        
        public Device Device { get; }
    }
    
    public sealed class OnSubscribeEventArgs
    {
        public OnSubscribeEventArgs(UserDevice userDevice)
        {
            UserDevice = userDevice;
        }
        
        public UserDevice UserDevice { get; }
    }
}