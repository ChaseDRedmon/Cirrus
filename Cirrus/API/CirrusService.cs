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
    public interface ICirrusService : IDisposable
    {
        /// <summary>
        ///     Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="query"> Ambient Weather API query </param>
        /// <param name="macAddress">The weather station MAC Address</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns> JSON <see cref="String"/> from the Ambient Weather API</returns>
        public Task<ServiceResponse<string>> Fetch(string query, string? macAddress = default,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        ///     Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="macAddress">The weather station MAC Address</param>
        /// <param name="query"> Ambient Weather API query </param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns> A deserialized JSON <see cref="IReadOnlyCollection{T}"/> response from the Ambient Weather API</returns>
        public Task<ServiceResponse<IEnumerable<T>?>> Fetch<T>(string query, string? macAddress = default, 
            CancellationToken cancellationToken = default);
    }

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
        
        public async Task<ServiceResponse<IEnumerable<T>?>> Fetch<T>(string query, string? macAddress = default, 
            CancellationToken cancellationToken = default)
        {
            var path = ConstructUri(macAddress, query);
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            
            using var responseMessage  = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var model = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(stream, new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            }, cancellationToken: cancellationToken);

            return ServiceResponse.Ok(model);
        }
        
        public async Task<ServiceResponse<string>> Fetch(string query, string? macAddress = default, 
            CancellationToken cancellationToken = default)
        {
            var path = ConstructUri(macAddress, query);
            var request = new HttpRequestMessage(HttpMethod.Get, path);

            using var responseMessage  = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            var jsonString = await reader.ReadToEndAsync();
            
            return ServiceResponse.Ok(jsonString);
        }
        
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