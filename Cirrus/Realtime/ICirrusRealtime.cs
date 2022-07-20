using System;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Models;

namespace Cirrus.Realtime;

/// <summary>
/// A websocket.io wrapper for interfacing with the Ambient Weather Websocket Server
/// </summary>
public interface ICirrusRealtime : IDisposable, IAsyncDisposable
{
    event EventHandler<OnSubscribeEventArgs> Subscribed;
    event EventHandler<OnDataReceivedEventArgs> DataReceived;
    event EventHandler Connected;
    event EventHandler Disconnected;
    
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