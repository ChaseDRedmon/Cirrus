using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cirrus.API
{
    /// <summary>
    /// Cirrus HTTP Client Service for sending REST Requests to the Ambient Weather Server
    /// </summary>
    public interface ICirrusService : IDisposable
    {
        /// <summary>
        /// Submits a request to the Ambient Weather API
        /// </summary>
        /// <typeparam name="T"> The type that resulting JSON will be deserialized to.</typeparam>
        /// <param name="query"> Ambient Weather API query.</param>
        /// <param name="macAddress">The weather station MAC Address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns> A deserialized JSON <see cref="IEnumerable{T}"/> response from the Ambient Weather API.</returns>
        Task<ServiceResponse<IEnumerable<T>?>> Fetch<T>(string query, string? macAddress = default, CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        /// Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="query"> Ambient Weather API query.</param>
        /// <param name="macAddress">The weather station MAC Address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns> JSON <see cref="string"/> from the Ambient Weather API.</returns>
        Task<ServiceResponse<string>> Fetch(string query, string? macAddress = default, CancellationToken cancellationToken = default);

        // Might end up exposing this in a future version if the users want to get raw Memory<char> types back
        /// <summary>
        /// Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="macAddress">The weather station MAC Address</param>
        /// <param name="query"> Ambient Weather API query </param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        // Task<Memory<char>> FetchMemory(string query, string? macAddress = default, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc />
    public sealed class CirrusService : ICirrusService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CirrusService> _logger;
        private bool _disposed;

        public CirrusService(HttpClient httpClient, ILogger<CirrusService>? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? NullLogger<CirrusService>.Instance;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<IEnumerable<T>?>> Fetch<T>(string query, string? macAddress = default, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            // var jsonResult = await FetchFromMemory(query, macAddress, cancellationToken);
            var path = ConstructUri(macAddress, query);
            var request = new HttpRequestMessage(HttpMethod.Get, path);

            using var responseMessage = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var model = JsonSerializer.Deserialize<IEnumerable<T>>(stream, new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });

            return ServiceResponse.Ok(model);
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<string>> Fetch(string query, string? macAddress = default, CancellationToken cancellationToken = default)
        {
            // var jsonResult = await FetchFromMemory(query, macAddress, cancellationToken);
            var path = ConstructUri(macAddress, query);
            var request = new HttpRequestMessage(HttpMethod.Get, path);

            using var responseMessage = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            var jsonResult = await reader.ReadToEndAsync();
            return ServiceResponse.Ok(jsonResult);
        }

        /// <inheritdoc />
        /// I might play with this a bit more in the future
        /*private async Task<Memory<char>> FetchFromMemory(string query, string? macAddress = default, CancellationToken cancellationToken = default)
        {
            // Hmm, something with this code is broken and I'm not entirely sure what it is. 
            // It doesn't seem to contain the full json response because when deserialization happens, Json Serializer throws an exception saying theres no JSON tokens
            var path = ConstructUri(macAddress, query);
            var request = new HttpRequestMessage(HttpMethod.Get, path);

            using var responseMessage = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            var memory = default(Memory<char>);
            await reader.ReadBlockAsync(memory, cancellationToken);

            return memory;
        }*/

        private string ConstructUri(string? macAddress, string query)
        {
            const string path = "/v1/devices/";
            return string.Concat(path, macAddress, query);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Release managed resources
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                // Release unmanaged resources
            }

            _disposed = true;
        }

        ~CirrusService()
        {
            Dispose(false);
        }
    }
}