namespace Cirrus.Models;

using System.Collections.Generic;

/// <summary>
/// Stores configuration values
/// </summary>
/// <param name="MacAddress">Weather Station MAC Address. Found Here: https://ambientweather.net/devices</param>
/// <param name="ApiKey">Ambient Weather API Key. Found Here: https://ambientweather.net/account</param>
/// <param name="ApplicationKey">Account Application Key. Found Here: https://ambientweather.net/account</param>
public sealed class CirrusConfig
{
    public string? MacAddress { get; set; }
    public List<string> ApiKeys { get; set; } = null!;
    public string ApplicationKey { get; set; } = null!;
}