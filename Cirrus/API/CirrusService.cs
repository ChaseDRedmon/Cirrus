using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Infrastructure;
using Cirrus.Models;
using Serilog;

namespace Cirrus.API
{
    public interface ICirrusService
    {
        /// <summary>
        ///     Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="query"> Ambient Weather API query </param>
        /// <param name="macAddress">The weather station MAC Address</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns> A deserialized JSON <see cref="IReadOnlyCollection{T?}"/> response from the Ambient Weather API</returns>
        public Task<ServiceResponse<IEnumerable<Device>>> Fetch(string query, string? macAddress = default,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        ///     Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="macAddress">The weather station MAC Address</param>
        /// <param name="query"> Ambient Weather API query </param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns> JSON <see cref="String"/> from the Ambient Weather API</returns>
        ///public Task<ServiceResponse<string>> Fetch(string query, string? macAddress = default, CancellationToken cancellationToken = default);
    }

    public sealed class CirrusService : ICirrusService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger? _log;
        private bool _disposed;

        public CirrusService(HttpClient httpClient, ILogger? log = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _log = log;
        }
        
        public async Task<ServiceResponse<IEnumerable<Device>>> Fetch(string query, string? macAddress = default,
            CancellationToken cancellationToken = default)
        {
            var serviceResponse = await FetchCore(query, macAddress, cancellationToken);
            if (serviceResponse.Failure)
                return ServiceResponse.Fail<IEnumerable<Device>?>(serviceResponse.ErrorMessage);
            
            //await using var stream = serviceResponse.Value;
            //var model = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(stream, cancellationToken: cancellationToken);
            return ServiceResponse.Ok(serviceResponse.Value);
        }
        
        /*public async Task<ServiceResponse<string>> Fetch(string query, string? macAddress = default, CancellationToken cancellationToken = default)
        {
            var serviceResponse = await FetchCore(query, macAddress, cancellationToken);

            if (serviceResponse.Failure)
                return ServiceResponse.Fail<string>(serviceResponse.ErrorMessage);

            await using var stream = serviceResponse.Value;
            using var reader = new StreamReader(stream);
            var result = await reader.ReadToEndAsync();
            
            return ServiceResponse.Ok(result);
        }*/

        private async Task<ServiceResponse<IEnumerable<Device>>> FetchCore(string query, string? macAddress = default,
            CancellationToken cancellationToken = default)
        {
            var path = ConstructUri(macAddress, query);
            
            // Get and return a JSON stream from the Ambient Weather API
            using var result = await _httpClient.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

            var streamReader = new StreamReader(stream);
            var jsonResponse = await streamReader.ReadToEndAsync();
            
            Log.Information(jsonResponse);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            await using var writer = new StreamWriter(Path.Combine(desktopPath, "A.json"));
            await writer.WriteAsync(jsonResponse);
            writer.Close();
            
            var model = await JsonSerializer.DeserializeAsync<IEnumerable<Device>>(stream, cancellationToken: cancellationToken);
            
            return ServiceResponse.Ok(model);
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