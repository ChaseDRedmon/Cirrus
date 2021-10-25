using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cirrus.Models
{
    public sealed record Geo
    {
        /// <summary>
        /// The Type of Geo Coordinates. i.e. "Point"
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; init; }
        
        /// <summary>
        /// A list of doubles containing the lat/lon coordinates
        /// coordinates[0] is longitude
        /// coordinates[1] is latitude
        /// </summary>
        [JsonPropertyName("coordinates")]
        public IReadOnlyList<double>? Coordinates { get; init; }
    }
    
    public sealed record Coords2
    {
        /// <summary>
        /// Latitude of the weather station
        /// </summary>
        [JsonPropertyName("lat")]
        public double Latitude { get; init; }
        
        /// <summary>
        /// Longitude of the weather station
        /// </summary>
        [JsonPropertyName("lon")]
        public double Longitude { get; init; }
    }
    
    public sealed record Coords
    {
        /// <summary>
        /// Geographic coordinates of the weather station
        /// </summary>
        public Coords2? Coord2 { get; init; }
        
        /// <summary>
        /// Address
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; init; }
        
        /// <summary>
        /// City
        /// </summary>
        [JsonPropertyName("location")]
        public string? Location { get; init; }
        
        /// <summary>
        /// Elevation above sea-level in meters
        /// </summary>
        [JsonPropertyName("elevation")]
        public double? Elevation { get; init; }
        
        /// <summary>
        /// Geographic coordinates of the station
        /// </summary>
        [JsonPropertyName("geo")]
        public Geo? Geo { get; init; }
    }
    
    // Root myDeserializedClass = JsonSerializer.Deserialize<UserDevice>(myJsonResponse);
    public sealed record Info
    {
        /// <summary>
        /// The name of the weather station configured in the AmbientWeather dashboard
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } 

        /// <summary>
        /// City Location
        /// </summary>
        [JsonPropertyName("coords")]
        public Coords? Coords { get; init; }
    }

    public sealed record UserDevice
    {
        /// <summary>
        /// Weather Station Mac Address
        /// </summary>
        [JsonPropertyName("macAddress")]
        public string? MacAddress { get; init; } 

        /// <summary>
        /// Instance of <see cref="Info"/> class
        /// </summary>
        [JsonPropertyName("info")]
        public Info? Info { get; init; } 

        /// <summary>
        /// Instance of <see cref="Device"/> class
        /// </summary>
        [JsonPropertyName("lastData")]
        public Device? LastData { get; init; } 
        
        /// <summary>
        /// The API Key used for the subcribe command
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string? ApiKey { get; init; }
    }
    
    public sealed record Root
    {
        /// <summary>
        /// List of devices belonging to the user
        /// </summary>
        [JsonPropertyName("devices")]
        public IReadOnlyList<UserDevice>? Devices { get; init; }
        
        /// <summary>
        /// List of invalid API keys
        /// After sending the 'unsubscribe' command, ambient weather returns a list of invalid API keys
        /// </summary>4
        [JsonPropertyName("invalidApiKeys")]
        public IReadOnlyList<string>? InvalidAPIKeys { get; init; }
        
        /// <summary>
        /// The returned event type
        /// </summary>
        [JsonPropertyName("method")]
        public string? Method { get; init; }
    }
}