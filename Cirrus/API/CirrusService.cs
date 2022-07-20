namespace Cirrus.API;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.Infrastructure;
using Cirrus.Adapters;

/// <summary>
/// Cirrus HTTP Client Service for sending REST Requests to the Ambient Weather Server
/// </summary>
public interface ICirrusService
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
}

/// <inheritdoc />
public sealed class CirrusService : ICirrusService
{
    private readonly HttpClient _client;
    private readonly ICirrusLoggerAdapter<CirrusService>? _logger;

    public CirrusService(HttpClient client, ICirrusLoggerAdapter<CirrusService>? logger = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<T>?>> Fetch<T>(string query, string? macAddress = default, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        await using var stream = await FetchCore(query, macAddress, cancellationToken);

        var model = await JsonSerializer.DeserializeAsync<IEnumerable<T>>(stream, new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        }, cancellationToken);

        return ServiceResponse.Ok(model);
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<string>> Fetch(string query, string? macAddress = default, CancellationToken cancellationToken = default)
    {
        await using var stream = await FetchCore(query, macAddress, cancellationToken);
        using var reader = new StreamReader(stream);
        var jsonResult = await reader.ReadToEndAsync();
        return ServiceResponse.Ok(jsonResult);
    }

    private async Task<Stream> FetchCore(string query, string? macAddress = default, CancellationToken cancellationToken = default)
    {
        var path = ConstructUri(macAddress, query);
        var request = new HttpRequestMessage(HttpMethod.Get, path);

        var responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

#if NET5_0_OR_GREATER
        var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
        var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif

        return stream;
    }

    private string ConstructUri(string? macAddress, string query)
    {
        const string path = "/v1/devices/";
        return string.Concat(path, macAddress, query);
    }
}