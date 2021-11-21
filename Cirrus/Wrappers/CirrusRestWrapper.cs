using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.API;
using Cirrus.Helpers;
using Cirrus.Infrastructure;
using Cirrus.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Cirrus.Wrappers
{
    public interface ICirrusRestWrapper : IDisposable
    {
        /// <summary>
        /// Weather Stations MAC Address. Found Here: https://ambientweather.net/devices
        /// </summary>
        string? MacAddress { get; set; }
        
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
        Task<IEnumerable<Device>> FetchDeviceDataAsync(DateTimeOffset? endDate, int limit = 288, CancellationToken cancellationToken = default);

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
        Task<string> FetchDeviceDataAsJsonAsync(DateTimeOffset? endDate, int limit = 288, CancellationToken cancellationToken = default);
        
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
        /// <returns>Returns a JSON string wrapped in a <see cref="ServiceResponse{T}"/></returns>
        /// <returns>If data does not exist for a given day or an error is thrown, returns an empty string</returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if <see cref="ApiKey"/>,
        /// <see cref="ApplicationKey"/>, or <see cref="MacAddress"/> are null, empty, or whitespace
        /// </exception>
        Task<ServiceResponse<string>> FetchDeviceDataAsServiceResponse(DateTimeOffset? endDate, int limit = 288, CancellationToken cancellationToken = default);

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
        ///     Fetch a list of devices and device metadata associated with the user's account and the most recent weather data for each device
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token. <see cref="CancellationToken" /></param>
        /// <returns>Returns a string wrapped in a <see cref="ServiceResponse{T}"/></returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if <see cref="ApiKey"/> or
        /// <see cref="ApplicationKey"/> are null, empty, or whitespace
        /// </exception>
        Task<ServiceResponse<string>> FetchUserDevices(CancellationToken cancellationToken);

        /// <summary>
        /// Checks to see if there is data for the specified day; if <see cref="dateToCheck"/> is null, then the Ambient Weather API will return data for the current date
        /// </summary>
        /// <param name="dateToCheck">The date to check to see if we have data for</param>
        /// <param name="cancellationToken">Cancellation Token. <see cref="CancellationToken" /></param>
        /// <returns>Returns a bool: true - if data exists for that day; false - if it does not</returns>
        /// <exception cref="ArgumentException">Throws ArgumentException if <see cref="ApiKey"/> or
        /// <see cref="ApplicationKey"/> are null, empty, or whitespace
        /// </exception>
        Task<bool> DoesDeviceDataExist(DateTimeOffset? dateToCheck, CancellationToken cancellationToken);
    }

    public sealed class CirrusRestWrapper : ICirrusRestWrapper
    {
        private readonly ILogger<CirrusRestWrapper> _log;
        private readonly ICirrusService _service;

        /// <summary>
        /// Class constructor for rest wrapper
        /// </summary>
        /// <param name="options">Options containing MacAddress, API Key, and Application key needed for REST Requests</param>
        /// <param name="service">HTTP Client service</param>
        /// <param name="logger">Serilog logger</param>
        public CirrusRestWrapper(IOptions<CirrusConfig> options, ICirrusService service, ILogger<CirrusRestWrapper>? logger = null)
        {
            MacAddress = options.Value.MacAddress;
            ApiKey = options.Value.ApiKeys[0];
            ApplicationKey = options.Value.ApplicationKey;
            
            _service = service;
            _log = logger ?? NullLogger<CirrusRestWrapper>.Instance;
        }

        public string? MacAddress { get; set; }
        public string ApiKey { get; set; }
        public string ApplicationKey { get; set; }
        
        public async Task<IEnumerable<Device>> FetchDeviceDataAsync(DateTimeOffset? endDate, int limit = 288, CancellationToken cancellationToken = default)
        {
            _log.LogTrace("[FetchDeviceDataAsync] MacAddress: {MacAddress}, ApiKey: {ApiKey}, ApplicationKey: {ApplicationKey}", MacAddress, ApiKey, ApplicationKey);
            
            Check.IsNullOrWhitespace(MacAddress, nameof(MacAddress));
            Check.IsNullOrWhitespace(ApiKey, nameof(ApiKey));
            Check.IsNullOrWhitespace(ApplicationKey, nameof(ApplicationKey));
            
            if (limit <= 0)
                return Enumerable.Empty<Device>();
            
            // Build our parameters 
            var query = string.Concat("?apiKey=", ApiKey, "&applicationKey=", ApplicationKey, "&endDate=",
                endDate?.ToUniversalTime().ToUnixTimeMilliseconds(), "&limit=", limit);
            
            // Fetch the JSON string
            var serviceResponse = await _service.Fetch<Device>(query, MacAddress, cancellationToken);
            
            if (serviceResponse.Success)
                return serviceResponse.Value;
                
            _log.LogWarning("Unsuccessful API Response: \n {Response}", serviceResponse.ErrorMessage);
            return Enumerable.Empty<Device>();
        }
        
        public async Task<string> FetchDeviceDataAsJsonAsync(DateTimeOffset? endDate, int limit = 288, CancellationToken cancellationToken = default)
        {
            _log.LogTrace("[FetchDeviceDataAsJsonAsync] MacAddress: {MacAddress}, ApiKey: {ApiKey}, ApplicationKey: {ApplicationKey}", MacAddress, ApiKey, ApplicationKey);
            _log.LogTrace("[FetchDeviceDataAsJsonAsync] Date: {Date}, Limit: {Limit}", endDate, limit);
            
            var serviceResponse = await FetchDeviceDataAsServiceResponse(endDate, limit, cancellationToken);

            if (serviceResponse.Success) 
                return serviceResponse.Value;
            
            _log.LogWarning("Unsuccessful API Response: \n {Response}", serviceResponse.ErrorMessage);
            return string.Empty;
        }

        public async Task<ServiceResponse<string>> FetchDeviceDataAsServiceResponse(DateTimeOffset? endDate, int limit = 288, CancellationToken cancellationToken = default)
        {
            _log.LogTrace("[FetchDeviceDataAsJsonAsync] MacAddress: {MacAddress}, ApiKey: {ApiKey}, ApplicationKey: {ApplicationKey}", MacAddress, ApiKey, ApplicationKey);
            _log.LogTrace("[FetchDeviceDataAsJsonAsync] Date: {Date}, Limit: {Limit}", endDate, limit);
            
            // Check to see if all parameters have a non-null, non-blank/whitespace value
            Check.IsNullOrWhitespace(MacAddress);
            Check.IsNullOrWhitespace(ApiKey);
            Check.IsNullOrWhitespace(ApplicationKey);
            
            if (limit <= 0)
                return ServiceResponse.Fail<string>($"{nameof(limit)} must be greater than 0");
            
            // Build our parameters 
            var query = string.Concat("?apiKey=", ApiKey, "&applicationKey=", ApplicationKey, "&endDate=",
                endDate?.ToUniversalTime().ToUnixTimeMilliseconds(), "&limit=", limit);

            // Query the Ambient Weather API
            var serviceResponse = await _service.Fetch(query, MacAddress, cancellationToken);
            
            if(serviceResponse.Failure)
                _log.LogWarning("Unsuccessful API Response: \n {Response}", serviceResponse.ErrorMessage);

            return serviceResponse;
        }

        public async Task<IEnumerable<UserDevice>> FetchUserDevicesAsync(CancellationToken cancellationToken = default)
        {
            Check.IsNullOrWhitespace(ApiKey);
            Check.IsNullOrWhitespace(ApplicationKey);

            var query = string.Concat("?apiKey=", ApiKey, "&applicationKey=", ApplicationKey);
            var serviceResponse = await _service.Fetch<UserDevice>(query, cancellationToken: cancellationToken);

            if (serviceResponse.Success)
                return serviceResponse.Value;
            
            _log.LogWarning("Unsuccessful API Response: \n {Response}", serviceResponse.ErrorMessage);
            return Enumerable.Empty<UserDevice>();
        }
        
        public async Task<string> FetchUserDevicesAsJsonAsync(CancellationToken cancellationToken = default)
        {
            // Check to see if all parameters have a non-null, non-blank/whitespace value
            var serviceResponse = await FetchUserDevices(cancellationToken);

            if (serviceResponse.Success) 
                return serviceResponse.Value;
            
            _log.LogWarning("Unsuccessful API Response: \n {Response}", serviceResponse.ErrorMessage);
            return string.Empty;
        }
        
        public async Task<ServiceResponse<string>> FetchUserDevices(CancellationToken cancellationToken = default)
        {
            // Check to see if all parameters have a non-null, non-blank/whitespace value
            Check.IsNullOrWhitespace(ApiKey);
            Check.IsNullOrWhitespace(ApplicationKey);

            var query = string.Concat("?apiKey=", ApiKey, "&applicationKey=", ApplicationKey);
            var serviceResponse = await _service.Fetch(query, cancellationToken: cancellationToken);
            
            if(serviceResponse.Failure)
                _log.LogWarning("Unsuccessful API Response: \n {Response}", serviceResponse.ErrorMessage);

            return serviceResponse;
        }

        public async Task<bool> DoesDeviceDataExist(DateTimeOffset? dateToCheck, CancellationToken cancellationToken = default)
        {
            // TODO: We really need a better way to handle and communicate faults to the user
            var serviceResponse = await FetchDeviceDataAsServiceResponse(dateToCheck, 1, cancellationToken);
            return !serviceResponse.Success || !serviceResponse.IsEmpty;
        }

        public void Dispose()
        {
            _service.Dispose();
        }
    }
}