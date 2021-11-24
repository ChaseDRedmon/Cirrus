using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cirrus.Extensions;
using Cirrus.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cirrus
{
    /// <summary>
    /// A wrapper class providing to fetch data between two dates or a number of days prior to the current time
    /// </summary>
    public interface ICirrusWrapper : IDisposable
    {
        /// <summary>
        /// Fetch the device's history between the start date and the end date, relative to the UTC timezone
        /// </summary>
        /// <param name="startDate">
        /// The start date where weather events will start being collected.
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
        /// <param name="token">Cancellation token.</param>
        /// <returns>Returns an <see cref="IAsyncEnumerable{IEnumerable}"/>.</returns>
        /// <exception cref="ArgumentException">When <see cref="endDate"/> is less than <see cref="startDate"/>.</exception>
        IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sliceTheListFromTheBeginningOfTheList = false, int limit = 288, CancellationToken token = default);
        
        
        /// <inheritdoc cref="FetchDeviceHistory(DateTimeOffset?, DateTimeOffset?, bool, int, CancellationToken)"/>
        /// <typeparam name="T"> A POCO to Deserialize JSON too.</typeparam>
        IAsyncEnumerable<IEnumerable<T>> FetchDeviceHistory<T>(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sliceTheListFromTheBeginningOfTheList = false, int limit = 288, CancellationToken token = default)
            where T : class, new();
        
        /// <summary>
        /// Fetch the device's history between now and the number of days specified, relative to UTC
        /// </summary>
        /// <param name="numberOfDaysToGoBack">How many days we should go back in time to retrieve information from the weather station.</param>
        /// <param name="sliceTheListFromTheBeginningOfTheList">This works in conjunction with the <see cref="limit"/> parameter.
        /// The API returns elements sliced from most recent to least recent per day.
        /// This parameter specifies that we should slice data from the beginning of the day (least recent) to most recent (end of the day).</param>
        /// <param name="includeToday">Include Today's Weather Information in the return results.</param>
        /// <param name="limit">
        /// Returns the number of elements for each day, limited to a maximum of 288.
        /// If information is stored every 5 minutes, 288 events translates to one (1), 24 hour day.
        /// </param>
        /// <param name="token">Cancellation token.</param>
        /// <returns><see cref="IAsyncEnumerable{IEnumerable}"/>.</returns>
        /// <exception cref="ArgumentException">When the number of <see cref="numberOfDaysToGoBack"/>.Days is less than or equal to 0.</exception>
        IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(TimeSpan numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, CancellationToken token = default);
        
        /// <inheritdoc cref="FetchDeviceHistory(TimeSpan, bool, bool, int, CancellationToken)"/>
        /// <typeparam name="T"> A POCO to Deserialize JSON too.</typeparam>
        IAsyncEnumerable<IEnumerable<T>> FetchDeviceHistory<T>(TimeSpan numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, CancellationToken token = default)
            where T : class, new();

        /// <inheritdoc cref="FetchDeviceHistory(TimeSpan, bool, bool, int, CancellationToken)" />
        IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(int? numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, CancellationToken token = default);
        
        /// <inheritdoc cref="FetchDeviceHistory(TimeSpan, bool, bool, int, CancellationToken)" />
        /// <typeparam name="T"> A POCO to Deserialize JSON too.</typeparam>
        IAsyncEnumerable<IEnumerable<T>> FetchDeviceHistory<T>(int? numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, CancellationToken token = default)
            where T : class, new();
    }

    /// <inheritdoc />
    public sealed class CirrusWrapper : ICirrusWrapper
    {
        private readonly ICirrusRestWrapper _restWrapper;
        private readonly ILogger<CirrusWrapper> _log;

        /// <summary>
        /// Creates an instance of the <see cref="CirrusWrapper"/> class
        /// </summary>
        /// <param name="macAddress">Device Mac Address.</param>
        /// <param name="apiKey">Ambient Weather API Key.</param>
        /// <param name="applicationKey">Ambient Weather Application Key.</param>
        /// <remarks>
        /// You must supply non-null, non-whitespace, non-empty strings for the
        /// <see cref="apiKey"/>, <see cref="applicationKey"/>, and <see cref="macAddress"/>
        /// parameters when calling any function within the <see cref="CirrusWrapper"/> class.
        /// The RestWrapper will throw an ArgumentException if these values are null, empty, or whitespace
        /// </remarks>
        /// <returns> Return an instance of <see cref="ICirrusWrapper"/>.</returns>
        public static ICirrusWrapper Create(string macAddress, List<string> apiKey, string applicationKey)
        {
            var services = new ServiceCollection();
            services.AddCirrusServices(options =>
            {
                options.MacAddress = macAddress;
                options.ApiKeys = apiKey;
                options.ApplicationKey = applicationKey;
            });

            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<ICirrusWrapper>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CirrusWrapper"/> class.
        /// </summary>
        /// <param name="restWrapper">The ICirrus Rest wrapper instance injected via container.</param>
        /// <param name="logger">MEL ILogger instance.</param>
        public CirrusWrapper(ICirrusRestWrapper restWrapper, ILogger<CirrusWrapper>? logger = null)
        {
            _restWrapper = restWrapper;
            _log = logger ?? NullLogger<CirrusWrapper>.Instance;
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sliceTheListFromTheBeginningOfTheList = false, int limit = 288, CancellationToken token = default)
        {
            _log.LogTrace("Fetching device history");
            _log.LogDebug("Start Date: {StartDate}, End Date: {EndDate}, Slice: {SliceList}, Limit: {Limit}", startDate, endDate, sliceTheListFromTheBeginningOfTheList, limit);
            
            return FetchDeviceHistory<Device>(startDate, endDate, sliceTheListFromTheBeginningOfTheList, limit, token);
        }

        public IAsyncEnumerable<IEnumerable<T>> FetchDeviceHistory<T>(DateTimeOffset? startDate, DateTimeOffset? endDate, bool sliceTheListFromTheBeginningOfTheList = false, int limit = 288, CancellationToken token = default) where T : class, new()
        {
            _log.LogTrace("Fetching device history");
            _log.LogDebug("Start Date: {StartDate}, End Date: {EndDate}, Slice: {SliceList}, Limit: {Limit}", startDate, endDate, sliceTheListFromTheBeginningOfTheList, limit);
            
            endDate ??= DateTimeOffset.UtcNow;

            if (endDate < startDate)
                throw new ArgumentException($"{nameof(endDate)} must be greater than {nameof(startDate)}");

            var daysBetween = endDate - startDate;
            return FetchDeviceHistory<T>(daysBetween?.Days, sliceTheListFromTheBeginningOfTheList, true, limit, token);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(TimeSpan numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, CancellationToken token = default)
        {
            return FetchDeviceHistory<Device>(numberOfDaysToGoBack, sliceTheListFromTheBeginningOfTheList, includeToday, limit, token);
        }

        public IAsyncEnumerable<IEnumerable<T>> FetchDeviceHistory<T>(TimeSpan numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, CancellationToken token = default)
            where T : class, new()
        {
            _log.LogTrace("Fetching device history");
            _log.LogDebug("Days: {Days}, Slice: {Slice}, Include Today?: {Include}, Limit: {Limit}", numberOfDaysToGoBack, sliceTheListFromTheBeginningOfTheList, includeToday, limit);

            if (numberOfDaysToGoBack.Days <= 0)
                throw new ArgumentException("Value must be greater than or equal to 1", nameof(numberOfDaysToGoBack));

            return FetchDeviceHistory<T>(numberOfDaysToGoBack.Days, sliceTheListFromTheBeginningOfTheList, includeToday, limit, token);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IEnumerable<Device>> FetchDeviceHistory(int? numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, CancellationToken token = default)
        {
            return FetchDeviceHistory<Device>(numberOfDaysToGoBack, sliceTheListFromTheBeginningOfTheList, includeToday, limit, token);
        }

        public async IAsyncEnumerable<IEnumerable<T>> FetchDeviceHistory<T>(int? numberOfDaysToGoBack, bool sliceTheListFromTheBeginningOfTheList = false, bool includeToday = true, int limit = 288, [EnumeratorCancellation] CancellationToken token = default)
            where T : class, new()
        {
            _log.LogTrace("Fetching device history");
            _log.LogDebug("Days: {Days}, Slice: {Slice}, Include Today?: {Include}, Limit: {Limit}", numberOfDaysToGoBack, sliceTheListFromTheBeginningOfTheList, includeToday, limit);

            if (numberOfDaysToGoBack <= 0)
                throw new ArgumentException("Value must be greater than or equal to 1", nameof(numberOfDaysToGoBack));

            if (limit <= 0)
                yield return Enumerable.Empty<T>();

            var queryLimit = limit;

            /* Ambient weather always places the most recent event as the first element in the list.
            Older events (from earlier in the day) are at the "bottom" of the list, meaning we use TakeLast to fetch from the start of the day
            We need to query the highest amount that we can (which is 288 elements) to fetch the full day, so that we can work from the bottom up
            We don't know how many elements are actually returned until we query */
            if (sliceTheListFromTheBeginningOfTheList)
                limit = 288;

            var queryDate = includeToday ? DateTimeOffset.UtcNow : DateTimeOffset.UtcNow.AddDays(-1);

            for (var i = 0; i <= numberOfDaysToGoBack; ++i)
            {
                var result = await _restWrapper.FetchDeviceDataAsync<T>(queryDate, limit, token);

                yield return sliceTheListFromTheBeginningOfTheList ? result.TakeLast(queryLimit) : result;
                queryDate = queryDate.AddDays(-1);
            }
        }

        public void Dispose()
        {
            _restWrapper.Dispose();
        }
    }
}