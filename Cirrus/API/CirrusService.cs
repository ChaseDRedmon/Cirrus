using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cirrus.API
{
    public interface ICirrusService
    {
        Task<HttpResponseMessage> QueryAmbientWeatherApiAsync(string path, string query,
            CancellationToken cancellationToken);
    }

    public sealed class CirrusService : ICirrusService
    {
        private readonly HttpClient _client = new();
        private readonly Uri _baseAddress = new("https://api.ambientweather.net/");
        
        /// <summary>
        ///     Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="path">API Path: "v1/devices" || "v1/devices/<see cref="MacAddress"/>"</param>
        /// <param name="query">API Query</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="HttpResponseMessage"/>HTTP Response from the Ambient Weather API</returns>
        public async Task<HttpResponseMessage> QueryAmbientWeatherApiAsync(string path, string query,
            CancellationToken cancellationToken)
        {
            // Build the full API Uri
            var builder = new UriBuilder
            {
                Scheme = _baseAddress.Scheme,
                Host = _baseAddress.Host,
                Path = path,
                Query = query
            };

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            request.Headers.TryAddWithoutValidation("Content-Type", "application/json");

            // Get and return a JSON string from the Ambient Weather API
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            return response;
        }
    }
}