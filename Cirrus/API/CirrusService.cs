using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Cirrus.API
{
    public interface ICirrusService
    {
        /// <summary>
        ///     Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="macAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="query"></param>
        /// <returns><see cref="HttpResponseMessage"/>HTTP Response from the Ambient Weather API</returns>
        Task<string> SendRequestAsync(string query, string? macAddress = default, CancellationToken cancellationToken = default);
    }

    public sealed class CirrusService : ICirrusService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger? _log;
        private bool _disposed;

        public CirrusService(HttpClient httpClient, ILogger? log = null)
        {
            _httpClient = httpClient;
            _log = log;
        }

        /// <inheritdoc/>
        public async Task<string> SendRequestAsync(string query, string? macAddress = default,
            CancellationToken cancellationToken = default)
        {
            var uri = ConstructUri(macAddress, query);
            
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            
            // Get and return a JSON string from the Ambient Weather API
            using var result = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await result.Content.ReadAsStringAsync(cancellationToken);
                case HttpStatusCode.Unauthorized:
                    _log?.Warning("Unauthorized credentials");
                    return string.Empty;
                case HttpStatusCode.TooManyRequests:
                    _log?.Warning("Too many requests made within one (1) second");
                    return string.Empty;
                default:
                    _log?.Error("Error: Unsuccessful API Response: \n HTTP: {StatusCode} - {Content}", result.StatusCode, result.Content.ReadAsStringAsync(cancellationToken));
                    break;
            }
            
            return string.Empty;
        }
        
        private Uri ConstructUri(string? macAddress, string query)
        { 
            var baseAddress = new Uri("https://api.ambientweather.net/");
            var path = "v1/devices/";

            if (!string.IsNullOrWhiteSpace(macAddress))
            {
                path += macAddress;
            }
            
            // Build the full API Uri
            var builder = new UriBuilder
            {
                Scheme = baseAddress.Scheme,
                Host = baseAddress.Host,
                Path = path,
                Query = query
            };

            return builder.Uri;
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