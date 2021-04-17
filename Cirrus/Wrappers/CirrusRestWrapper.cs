using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Helpers;
using Cirrus.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Cirrus.Wrappers
{
    public interface ICirrusRestWrapper
    {
        /// <summary>
        /// Weather Stations MAC Address. Found Here: https://ambientweather.net/devices
        /// </summary>
        string MacAddress { get; set; }
        
        /// <summary>
        /// Account API Key. Found Here: https://ambientweather.net/account
        /// </summary>
        string ApiKey { get; set; }
        
        /// <summary>
        /// Account Application Key. Found Here: https://ambientweather.net/account
        /// </summary>
        string ApplicationKey { get; set; }

        /// <summary>
        ///     Fetches a Weather Station's data based on its MAC Address from the Ambient Weather API
        /// </summary>
        /// <param name="endDate">Date to query information for; if <see cref="endDate"/> is null, then the Ambient Weather API will return data for the current date</param>
        /// <param name="cancellationToken">Cancellation Token. <see cref="CancellationToken" /></param>
        /// <param name="limit">
        ///     The amount of items to return. Maximum is 288. Items are in 5 minute increments, meaning 288 items
        ///     is 1 day's worth of data.
        /// </param>
        /// <returns>Returns a <see cref="Device" /> object.</returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if <see cref="ApiKey"/>,
        /// <see cref="ApplicationKey"/>, or <see cref="MacAddress"/> are null, empty, or whitespace
        /// </exception>
        Task<IEnumerable<Device>> FetchDeviceDataAsync(DateTimeOffset? endDate, CancellationToken cancellationToken, int limit = 288);

        /// <summary>
        ///     Fetches a Weather Station's data based on its MAC Address from the Ambient Weather API
        /// </summary>
        /// <param name="endDate">
        /// Date to query information for; if <see cref="endDate"/> is null,
        /// then the Ambient Weather API will return data for the current date
        /// </param>
        /// <param name="cancellationToken">Cancellation Token. <see cref="CancellationToken" /></param>
        /// <param name="limit">
        ///     The amount of items to return. Maximum is 288. Items are in 5 minute increments, meaning 288 items
        ///     is 1 day's worth of data.
        /// </param>
        /// <returns>Returns a JSON string</returns>
        /// <returns>If data does not exist for a given day or an error is thrown, returns an empty string</returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if <see cref="ApiKey"/>,
        /// <see cref="ApplicationKey"/>, or <see cref="MacAddress"/> are null, empty, or whitespace
        /// </exception>
        Task<string> FetchDeviceDataAsJsonAsync(DateTimeOffset? endDate, CancellationToken cancellationToken, int limit = 288);

        /// <summary>
        ///     Fetch a list of devices and device metadata associated with the user's account and the most recent weather data for each device
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token. <see cref="CancellationToken" /></param>
        /// <returns>
        /// Returns an IEnumerable of <see cref="UserDevice" /> objects. 
        /// If the result does not have a success status code, returns an empty <see cref="IEnumerable{Device}"/>
        /// </returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if <see cref="ApiKey"/> or
        /// <see cref="ApplicationKey"/> are null, empty, or whitespace
        /// </exception>
        Task<IEnumerable<UserDevice>> FetchUserDevicesAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Fetch a list of devices and device metadata associated with the user's account and the most recent weather data for each device
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token. <see cref="CancellationToken" /></param>
        /// <returns>Returns a JSON string. </returns>
        /// <returns>If the result does not have a success status code, returns an empty string</returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if <see cref="ApiKey"/> or
        /// <see cref="ApplicationKey"/> are null, empty, or whitespace
        /// </exception>
        Task<string> FetchUserDevicesAsJsonAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Checks to see if there is data for the specified day; if <see cref="dateToCheck"/> is null, then the Ambient Weather API will return data for the current date
        /// </summary>
        /// <param name="dateToCheck">The date to check to see if we have data for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a bool: true - if data exists for that day; false - if it does not</returns>
        Task<bool> DoesDeviceDataExist(DateTimeOffset? dateToCheck, CancellationToken cancellationToken);
    }

    public sealed class CirrusRestWrapper : ICirrusRestWrapper
    {
        private static readonly HttpClient Client = new HttpClient();
        private static Uri BaseAddress { get; } = new Uri("https://api.ambientweather.net/");

        private readonly ILogger? _log;

        /// <summary>
        ///     Creates a new <see cref="CirrusRestWrapper" /> and initializes the base address for the Ambient Weather API
        /// </summary>
        public CirrusRestWrapper(string macAddress, string apiKey, string applicationKey, ILogger logger): this(macAddress, apiKey, applicationKey)
        {
            _log = logger.ForContext<CirrusRestWrapper>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="macAddress"></param>
        /// <param name="apiKey"></param>
        /// <param name="applicationKey"></param>
        public CirrusRestWrapper(string macAddress, string apiKey, string applicationKey)
        {
            MacAddress = macAddress;
            ApiKey = apiKey;
            ApplicationKey = applicationKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public CirrusRestWrapper(IOptions<CirrusConfig> options, ILogger logger)
        {
            MacAddress = options.Value.MacAddress;
            ApiKey = options.Value.ApiKey[0];
            ApplicationKey = options.Value.ApplicationKey;
            
            _log = logger.ForContext<CirrusRestWrapper>();
        }
        
        public string MacAddress { get; set; }
        public string ApiKey { get; set; }
        public string ApplicationKey { get; set; }

        /// <inheritdoc cref="FetchDeviceDataAsync" />
        public async Task<IEnumerable<Device>> FetchDeviceDataAsync(DateTimeOffset? endDate, CancellationToken cancellationToken, int limit = 288)
        {
            if (limit <= 0)
                return Enumerable.Empty<Device>();
            
            // Fetch the JSON string
            var json = await FetchDeviceDataAsJsonAsync(endDate, cancellationToken, limit);

            if (string.IsNullOrEmpty(json))
                return Enumerable.Empty<Device>();
            
            var data = JsonConvert.DeserializeObject<IEnumerable<Device>>(json);
            return data!;
        }

        /// <inheritdoc cref="FetchDeviceDataAsJsonAsync" />
        public async Task<string> FetchDeviceDataAsJsonAsync(DateTimeOffset? endDate, CancellationToken cancellationToken, int limit = 288)
        {
            // Check to see if all parameters have a non-null, non-blank/whitespace value
            Check.IsNullOrWhitespace(MacAddress);
            Check.IsNullOrWhitespace(ApiKey);
            Check.IsNullOrWhitespace(ApplicationKey);
            
            if (limit <= 0)
                return string.Empty;
            
            // Build our query
            var path = $"v1/devices/{MacAddress}";
            var query = $"?apiKey={ApiKey}&applicationKey={ApplicationKey}";

            query += $"&endDate={endDate?.ToUniversalTime().ToUnixTimeMilliseconds().ToString()}";
            query += $"&limit={limit.ToString()}";

            // Query the Ambient Weather API
            var responseMessage = await QueryAmbientWeatherApiAsync(path, query, cancellationToken);
            var content = await responseMessage.Content.ReadAsStringAsync();

            if (content.Length == 2)
            {
                return string.Empty;
            }
            
            if (responseMessage.IsSuccessStatusCode)
            {
                return content;
            }
            
            _log?.Error($"Error: Unsuccessful API Response: \n HTTP: {responseMessage.StatusCode} - {content}");
            return string.Empty;
        }

        /// <inheritdoc cref="FetchUserDevicesAsync" />
        public async Task<IEnumerable<UserDevice>> FetchUserDevicesAsync(CancellationToken cancellationToken)
        {
            // Fetch the JSON string
            var json = await FetchUserDevicesAsJsonAsync(cancellationToken);

            if (string.IsNullOrEmpty(json))
            {
                return Enumerable.Empty<UserDevice>();
            }

            var data = JsonConvert.DeserializeObject<IEnumerable<UserDevice>>(json);
            return data!;
        }

        public async Task<string> FetchUserDevicesAsJsonAsync(CancellationToken cancellationToken)
        {
            // Check to see if all parameters have a non-null, non-blank/whitespace value
            Check.IsNullOrWhitespace(ApiKey);
            Check.IsNullOrWhitespace(ApplicationKey);

            // Build our query
            const string path = "v1/devices";
            var query = $"?apiKey={ApiKey}&applicationKey={ApplicationKey}";

            // Query the Ambient Weather API
            var responseMessage = await QueryAmbientWeatherApiAsync(path, query, cancellationToken);
            var content = await responseMessage.Content.ReadAsStringAsync();
            
            if (responseMessage.IsSuccessStatusCode)
            {
                return content;
            }
            
            _log?.Error($"Error: Unsuccessful API Response: \n HTTP: {responseMessage.StatusCode} - {content}");
            return string.Empty;
        }
        
        public async Task<bool> DoesDeviceDataExist(DateTimeOffset? dateToCheck, CancellationToken cancellationToken)
        {
            var json = await FetchDeviceDataAsJsonAsync(dateToCheck, cancellationToken, 1);

            // The Ambient Weather API returns HTTP 200 and an empty JSON Array when data does not exist for a given day.
            return json.Length != 2;
        }

        /// <summary>
        ///     Submits a request to the Ambient Weather API
        /// </summary>
        /// <param name="path">API Path: "v1/devices" || "v1/devices/<see cref="MacAddress"/>"</param>
        /// <param name="query">API Query</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="HttpResponseMessage"/>HTTP Response from the Ambient Weather API</returns>
        private async Task<HttpResponseMessage> QueryAmbientWeatherApiAsync(string path, string query,
            CancellationToken cancellationToken)
        {
            // Build the full API Uri
            var builder = new UriBuilder
            {
                Scheme = BaseAddress.Scheme,
                Host = BaseAddress.Host,
                Path = path,
                Query = query
            };

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            request.Headers.TryAddWithoutValidation("Content-Type", "application/json");

            // Get and return a JSON string from the Ambient Weather API
            var response = await Client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            return response;
        }
    }
}