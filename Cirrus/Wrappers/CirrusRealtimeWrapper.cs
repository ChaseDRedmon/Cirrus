using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Cirrus.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
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

    public sealed class CirrusRealtime : ICirrusRealtime, IDisposable
    {
        private readonly ILogger? _log;
        
        private SocketIO? Client { get; set; }
        private static Uri BaseAddress { get; } = new Uri("https://dash2.ambientweather.net");
        private Timer Timer { get; set; }
        
        private CirrusConfig Options { get; }
        
        /// <inheritdoc cref="OnDataReceived"/>
        public event ICirrusRealtime.OnDataReceivedHandler OnDataReceived;
        
        /// <inheritdoc cref="OnSubscribe"/>
        public event ICirrusRealtime.OnSubcribeHandler OnSubscribe;

        public CirrusRealtime(IOptions<CirrusConfig> options, ILogger logger): this(options)
        {
            _log = logger.ForContext<CirrusRealtime>();
        }
        
        public CirrusRealtime(IOptions<CirrusConfig> options)
        {
            Options = options.Value;
        }

        public async Task OpenConnection()
        {
            var apiKeys = Options.ApiKey;
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

            Client.On("subscribed", OnInternalSubscribeEvent);
            Client.On("data", OnInternalDataEvent);

            Client.OnConnected += OnInternalConnectEvent;
            Client.OnDisconnected += OnInternalDisconnectEvent;
            
            _log?.Information($"Opening websocket connection: {BaseAddress}");
            await Client.ConnectAsync();
            
            _log?.Information($"Sending Subcribe Command: {BaseAddress}");
            await Client.EmitAsync("subscribe", apiKeys);

            Timer = new Timer { Interval = 10000 };
            Timer.Elapsed += KeepConnectionAlive;
            Timer.Start();

            await Task.Delay(-1);
        }

        public async Task Unsubscribe()
        {
            _log?.Information("Unsubscribing from the ambient weather websocket service");
            await Client?.EmitAsync("unsubscribe");
        }

        private void OnInternalDisconnectEvent(object sender, string e)
        {
            _log?.Information("API Disconnected");
            Timer.Stop();
        }

        private void OnInternalConnectEvent(object sender, EventArgs e)
        {
            _log?.Information("Connected to API");

            if (!Timer.Enabled)
            {
                Timer.Start();
            }
        }

        private void OnInternalSubscribeEvent(SocketIOResponse obj)
        {
            _log?.Information("Subscribed to service");

            var x = obj.GetValue().ToObject<UserDevice>();
            OnSubscribe.Invoke(this, new OnSubscribeEventArgs(x));
        }

        private void OnInternalDataEvent(SocketIOResponse obj)
        {
            _log?.Information("Received data event");

            var x = obj.GetValue().ToObject<Device>();
            OnDataReceived.Invoke(this, new OnDataReceivedEventArgs(x));
        }

        private async void KeepConnectionAlive(object source, ElapsedEventArgs e)
        {
            // This "ping" event emulates a keep-alive message to prevent the API from disconnecting
            _log?.Information("Sending ping keep-alive");
            await Client.EmitAsync("ping");
        }

        private void ReleaseUnmanagedResources()
        {
            Client.Off("subscribed");
            Client.Off("data");

            Client.OnConnected -= OnInternalConnectEvent;
            Client.OnDisconnected -= OnInternalDisconnectEvent;
            
            // Tell the API we are disconnecting
            Client.EmitAsync("disconnect").Wait();
            Client.DisconnectAsync().Wait();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Timer.Elapsed -= KeepConnectionAlive;
            
                Timer.Stop();
                Timer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CirrusRealtime()
        {
            Dispose(false);
        }
    }
    
    public class OnDataReceivedEventArgs
    {
        public OnDataReceivedEventArgs(Device device)
        {
            Device = device;
        }
        
        public Device Device { get; }
    }
    
    public class OnSubscribeEventArgs
    {
        public OnSubscribeEventArgs(UserDevice userDevice)
        {
            UserDevice = userDevice;
        }
        
        public UserDevice UserDevice { get; }
    }
}