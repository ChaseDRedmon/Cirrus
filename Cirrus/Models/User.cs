using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Models
{
    public class Geo
    {
        /// <summary>
        /// The Type of Geo Coordinates. i.e. "Point"
        /// </summary>
        [JsonProperty("type")]
        public string? Type { get; set; }
        
        /// <summary>
        /// A list of doubles containing the lat/lon coordinates
        /// coordinates[0] is longitude
        /// coordinates[1] is latitude
        /// </summary>
        [JsonProperty("coordinates")]
        public List<double>? Coordinates { get; set; }
    }
    
    public class Coords2
    {
        /// <summary>
        /// Latitude of the weather station
        /// </summary>
        [JsonProperty("lat")]
        public double Latitude { get; set; }
        
        /// <summary>
        /// Longitude of the weather station
        /// </summary>
        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }
    
    public class Coords
    {
        /// <summary>
        /// Geographic coordinates of the weather station
        /// </summary>
        public Coords2? Coord2 { get; set; }
        
        /// <summary>
        /// Address
        /// </summary>
        [JsonProperty("address")]
        public string? Address { get; set; }
        
        /// <summary>
        /// City
        /// </summary>
        [JsonProperty("location")]
        public string? Location { get; set; }
        
        /// <summary>
        /// Elevation above sea-level in meters
        /// </summary>
        [JsonProperty("elevation")]
        public double? Elevation { get; set; }
        
        /// <summary>
        /// Geographic coordinates of the station
        /// </summary>
        [JsonProperty("geo")]
        public Geo? Geo { get; set; }
    }
    
    // Root myDeserializedClass = JsonSerializer.Deserialize<UserDevice>(myJsonResponse);
    public class Info
    {
        /// <summary>
        /// The name of the weather station configured in the AmbientWeather dashboard
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; } 

        /// <summary>
        /// City Location
        /// </summary>
        [JsonProperty("coords")]
        public Coords? Coords { get; set; }
    }

    public class UserDevice
    {
        /// <summary>
        /// Weather Station Mac Address
        /// </summary>
        [JsonProperty("macAddress")]
        public string? MacAddress { get; set; } 

        /// <summary>
        /// Instance of <see cref="Info"/> class
        /// </summary>
        [JsonProperty("info")]
        public Info? Info { get; set; } 

        /// <summary>
        /// Instance of <see cref="Device"/> class
        /// </summary>
        [JsonProperty("lastData")]
        public Device? LastData { get; set; } 
        
        /// <summary>
        /// The API Key used for the subcribe command
        /// </summary>
        [JsonProperty("apiKey")]
        public string? ApiKey { get; set; }
    }
    
    public class Root
    {
        /// <summary>
        /// List of devices belonging to the user
        /// </summary>
        [JsonProperty("devices")]
        public List<UserDevice>? Devices { get; set; }
        
        /// <summary>
        /// List of invalid API keys
        /// After sending the 'unsubscribe' command, ambient weather returns a list of invalid API keys
        /// </summary>4
        [JsonProperty("invalidApiKeys")]
        public List<string>? InvalidAPIKeys { get; set; }
        
        /// <summary>
        /// The returned event type
        /// </summary>
        [JsonProperty("method")]
        public string? Method { get; set; }
    }
}