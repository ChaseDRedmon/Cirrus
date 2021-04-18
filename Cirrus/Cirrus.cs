using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Cirrus
{
    public interface ICirrus
    {
        /// <summary>
        /// Fetch the device's history between the start date and the end date, relative to the UTC timezone
        /// </summary>
        /// <param name="startDate">
        /// The start date where weather events will start being collected.
        /// 
        /// </param>
        /// <param name="endDate">
        /// The end date where weather events will stop being collected.
        /// An end date of February 20th, 2021 will fetch February 19th, 00:05 - 23:55 UTC
        /// </param>
        /// <param name="sliceTheListFromTheBeginningOfTheList">This works in conjunction with the <see cref="limit"/> parameter.
        /// The API returns elements sliced from most recent to least recent per day.
        /// This parameter specifies that we should slice data from the beginning of the day (least recent) to most recent (end of the day)</param>
        /// <param name="limit">
        /// Returns the number of elements for each day, limited to a maximum of 288.
        /// If information is stored every 5 minutes, 288 events translates to one (1), 24 hour day.
        /// </param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Returns an <see cref="IAsyncEnumerable{IEnumerable}"/></returns>
        public IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(DateTimeOffset? startDate, DateTimeOffset? endDate, CancellationToken token, bool sliceTheListFromTheBeginningOfTheList = false, int limit = 288);

        /// <summary>
        /// Fetch the device's history between now and the number of days specified, relative to UTC
        /// </summary>
        /// <param name="numberOfDaysToGoBack">How many days we should go back in time to retrieve information from the weather station</param>
        /// <param name="sliceTheListFromTheBeginningOfTheList">This works in conjunction with the <see cref="limit"/> parameter.
        /// The API returns elements sliced from most recent to least recent per day.
        /// This parameter specifies that we should slice data from the beginning of the day (least recent) to most recent (end of the day)</param>
        /// <param name="includeToday">Include Today's Weather Information in the return results.</param>
        /// <param name="limit">
        /// Returns the number of elements for each day, limited to a maximum of 288.
        /// If information is stored every 5 minutes, 288 events translates to one (1), 24 hour day.
        /// </param>
        /// <param name="token">Cancellation token</param>
        /// <returns><see cref="IAsyncEnumerable{IEnumerable}"/></returns>
        public IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(TimeSpan numberOfDaysToGoBack, CancellationToken token, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288);

        /// <inheritdoc cref="FetchDeviceHistory(TimeSpan, CancellationToken, bool, bool, int)" />
        public IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(int numberOfDaysToGoBack, CancellationToken token, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288);
    }

    public class Cirrus : ICirrus
    {
        private readonly ICirrusRestWrapper _restWrapper;
        private readonly ILogger? _log;

        /// <summary>
        /// Creates an instance of the <see cref="Cirrus"/> class
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="applicationKey"></param>
        /// <param name="macAddress"></param>
        /// <param name="logger"></param>
        /// <remarks>
        /// You must supply non-null, non-whitespace, non-empty strings for the
        /// <see cref="apiKey"/>, <see cref="applicationKey"/>, and <see cref="macAddress"/>
        /// parameters when calling any function within the <see cref="Cirrus"/> class.
        /// The RestWrapper will throw an ArgumentException if these values are null, empty, or whitespace
        /// </remarks>
        public Cirrus(string apiKey, string applicationKey, string macAddress, ILogger logger): this(apiKey, applicationKey, macAddress)
        {
            _log = logger.ForContext<Cirrus>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="applicationKey"></param>
        /// <param name="macAddress"></param>
        public Cirrus(string apiKey, string applicationKey, string macAddress)
        {
            var services = new ServiceCollection();
            services.AddTransient<ICirrusRestWrapper>(x =>
                new CirrusRestWrapper(macAddress, apiKey, applicationKey));
            
            var provider = services.BuildServiceProvider();
            _restWrapper = provider.GetRequiredService<ICirrusRestWrapper>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="restWrapper"></param>
        /// <param name="logger"></param>
        public Cirrus(ICirrusRestWrapper restWrapper, ILogger logger)
        {
            _restWrapper = restWrapper;
            _log = logger.ForContext<Cirrus>();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(DateTimeOffset? startDate, DateTimeOffset? endDate, [EnumeratorCancellation] CancellationToken token, bool sliceTheListFromTheBeginningOfTheList = false, int limit = 288)
        {
            _log?.Information($"Fetching device history from: {startDate?.ToUniversalTime().ToString()} to {endDate?.ToUniversalTime().ToString()}");

            var current = startDate;
            
            var queryLimit = limit;

            if (sliceTheListFromTheBeginningOfTheList)
                limit = 288;

            // Walk the API 1 day at a time until we reach the end date
            while (current <= endDate)
            {
                var result = await _restWrapper.FetchDeviceDataAsync(current, token, limit);

                yield return sliceTheListFromTheBeginningOfTheList ? result.TakeLast(queryLimit) : result;

                current = current?.AddDays(1);
                
                // Wait 1.5 seconds before sending the next request, as the API allows 1 request per second
                await Task.Delay(1500);
            }
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(TimeSpan numberOfDaysToGoBack, CancellationToken token, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288)
        {
            if (numberOfDaysToGoBack.Days <= 0)
                throw new ArgumentException("Value must be greater than or equal to 1", nameof(numberOfDaysToGoBack));

            return FetchDeviceHistory(numberOfDaysToGoBack.Days, token, sliceTheListFromTheBeginningOfTheList, includeToday, limit);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(int numberOfDaysToGoBack, [EnumeratorCancellation] CancellationToken token, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288)
        {
            if(numberOfDaysToGoBack <= 0)
                throw new ArgumentException("Value must be greater than or equal to 1", nameof(numberOfDaysToGoBack));

            var queryLimit = limit;

            /* Ambient weather always places the most recent event as the first element in the list.
            Older events (from earlier in the day) are at the "bottom" of the list, meaning we use TakeLate to fetch from the start of the day
            We need to query the highest amount that we can (which is 288 elements) to fetch the full day, so that we can work from the bottom up
            We don't know how many elements are actually returned until we query */
            if (sliceTheListFromTheBeginningOfTheList)
                limit = 288;
            
            var queryDate = includeToday ? DateTime.UtcNow : DateTime.UtcNow.AddDays(-1);
            
            for (var i = 0; i <= numberOfDaysToGoBack; i++)
            {
                var result = await _restWrapper.FetchDeviceDataAsync(queryDate, token, limit);

                yield return sliceTheListFromTheBeginningOfTheList ? result.TakeLast(queryLimit) : result;
                
                queryDate = queryDate.AddDays(-1);

                // Wait 1.5 seconds before sending the next request, as the API allows 1 request per second
                await Task.Delay(1500);
            }
        }
    }
}