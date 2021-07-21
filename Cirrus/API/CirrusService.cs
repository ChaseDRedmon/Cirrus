using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Infrastructure;
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
        public Task<ServiceResponse<string>> SendRequestAsync(string query, string? macAddress = default, CancellationToken cancellationToken = default);

        internal Task<MemoryStream> Test(string query, string? macAddress = default, CancellationToken cancellationToken = default);
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
        public async Task<ServiceResponse<string>> SendRequestAsync(string query, string? macAddress = default,
            CancellationToken cancellationToken = default)
        {
            
            
            _log?.Debug("Returned status code: {StatusCode}", result.StatusCode);
            _log?.Verbose("JSON String: \n{JsonResult}", json);
            
            
            
            return ServiceResponse.Fail<string>($"HTTP: {result.StatusCode.ToString()} - {json}");
        }

        async Task<MemoryStream> ICirrusService.Test(string query, string? macAddress = default,
            CancellationToken cancellationToken = default)
        {
            var x = await (Fetch());
            
            x.Value.
        }

        private async Task<ServiceResponse<MemoryStream>> Fetch(string query, string? macAddress = default,
            CancellationToken cancellationToken = default)
        {
            var uri = ConstructUri(macAddress, query);
            
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            
            // Get and return a JSON string from the Ambient Weather API
            using var result = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var statusCode = result.StatusCode;            
            
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    var mstream = new MemoryStream();
                    await result.Content.CopyToAsync(mstream, cancellationToken);
                    return ServiceResponse.Ok(mstream);
                case HttpStatusCode.Unauthorized:
                    return ServiceResponse.Fail<MemoryStream>("Unauthorized credentials");
                case HttpStatusCode.TooManyRequests:
                    return ServiceResponse.Fail<MemoryStream>("Too many requests made within one (1) second");
            }

            return ServiceResponse.Fail<MemoryStream>("Unhandled HTTP Status Code");
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

            _log?.Debug("Scheme: {Scheme}\nHost: {Host}\nPath: {Path}\nQuery: {Query}\nFull Uri: {Uri}", 
                builder.Scheme, builder.Host, builder.Path, builder.Query, builder.Uri);
            
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