#pragma warning disable SA1623
using System;
using System.Text.Json.Serialization;

namespace Cirrus.Models
{
    public sealed record Device
    {
        // These fields below are fields that I have personally retrieved from my Ambient Weather Station (WS-2902A/B)

        /// <summary>
        /// Epoch (Unix) time from 1/1/1970 (measured in milliseconds according to ambient weather docs).
        /// </summary>
        [JsonPropertyName("dateutc")]
        public long? EpochMilliseconds { get; init; }

        /// <summary>
        /// Indoor Temperature in Fahrenheit reported by the Base Station.
        /// </summary>
        [JsonPropertyName("tempinf")]
        public double? IndoorTemperatureFahrenheit { get; init; }

        /// <summary>
        /// Indoor Humidity reported by the Base Station.
        /// </summary>
        [JsonPropertyName("humidityin")]
        public int? IndoorHumidity { get; init; }

        /// <summary>
        /// Relative Barometric Pressure in inches of mercury (in-HG) reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("baromrelin")]
        public double? RelativeBarometricPressure { get; init; }

        /// <summary>
        /// Absolute Barometric Pressure in inches of mercury (in-HG) reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("baromabsin")]
        public double? AbsoluteBarometricPressure { get; init; } 

        /// <summary>
        /// Outdoor Temperature in Fahrenheit reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("tempf")]
        public double? OutdoorTemperatureFahrenheit { get; init; }

        /// <summary>
        /// A battery indicator reported by the Outdoor Sensor Array.
        /// A value of 1 represents an 'OK' battery level.
        /// A value of 0 represents a 'low' battery level.
        ///
        /// For Meteobridge Users: the above value are flipped. See below.
        /// A value of 0 represents an 'OK' battery level.
        /// A value of 1 represents a 'low' battery level.
        /// </summary>
        [JsonPropertyName("battout")]
        public int? BatteryLowIndicator { get; init; }

        /// <summary>
        /// The outdoor humidity reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("humidity")]
        public int? OutdoorHumidity { get; init; }

        /// <summary>
        /// Wind Direction reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("winddir")]
        public int? WindDirection { get; init; }

        /// <summary>
        /// Wind Speed in Miles Per Hour reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("windspeedmph")]
        public double? WindSpeedMph { get; init; }

        /// <summary>
        /// Wind Gust in Miles Per Hour reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("windgustmph")]
        public double? WindGustMph { get; init; }

        /// <summary>
        /// The maximum windspeed from a wind gust for that day.
        /// </summary>
        [JsonPropertyName("maxdailygust")]
        public double? MaxDailyGust { get; init; }

        /// <summary>
        /// Hourly Rainfall in Inches.
        /// </summary>
        [JsonPropertyName("hourlyrainin")]
        public double? HourlyRainfall { get; init; }

        /// <summary>
        /// Current event's rainfall in inches.
        /// </summary>
        [JsonPropertyName("eventrainin")]
        public double? EventRainfall { get; init; }

        /// <summary>
        /// Daily Rainfall in Inches.
        /// </summary>
        [JsonPropertyName("dailyrainin")]
        public double? DailyRainfall { get; init; }

        /// <summary>
        /// Weekly rainfall in inches.
        /// </summary>
        [JsonPropertyName("weeklyrainin")]
        public double? WeeklyRainfall { get; init; }

        /// <summary>
        /// Monthly rainfall in inches.
        /// </summary>
        [JsonPropertyName("monthlyrainin")]
        public double? MonthlyRainfall { get; init; }

        /// <summary>
        /// Yearly rainfall in inches.
        /// </summary>
        [JsonPropertyName("yearlyrainin")]
        public double? YearlyRainfall { get; init; }

        /// <summary>
        /// Total rainfall recorded by sensor in inches.
        /// </summary>
        [JsonPropertyName("totalrainin")]
        public double? TotalRainfall { get; init; }

        /// <summary>
        /// Solar Radiation measured in Watts Per Meter^2 (W/m^2) reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("solarradiation")]
        public double? SolarRadiation { get; init; }

        /// <summary>
        /// Ultra-violet radiation index reported by the Outdoor Sensor Array.
        /// </summary>
        [JsonPropertyName("uv")]
        public int? UltravioletRadiationIndex { get; init; }

        /// <summary>
        /// Feels Like Temperature.
        /// If temperature is less than 50ºF => Wind Chill;
        /// If temperature is greater than 68ºF => Heat Index (calculated on server).
        /// </summary>
        [JsonPropertyName("feelsLike")]
        public double? OutdoorFeelsLikeTemperatureFahrenheit { get; init; }

        /// <summary>
        /// Dew Point Temperature in Fahrenheit.
        /// </summary>
        [JsonPropertyName("dewPoint")]
        public double? DewPointFahrenheit { get; init; }

        /// <summary>
        /// Indoor Feels Like Temperature in Fahrenheit reported by the Base Station.
        /// </summary>
        [JsonPropertyName("feelsLikein")]
        public double? IndoorFeelsLikeTemperatureFahrenheit { get; init; }

        /// <summary>
        /// Indoor Dew Point Temperature in Fahrenheit reported by the Base Station.
        /// </summary>
        [JsonPropertyName("dewPointin")]
        public double? IndoorDewPointTemperatureFahrenheit { get; init; }

        /// <summary>
        /// Last DateTime recorded where <see cref="HourlyRainfall"/> was > 0 inches.
        /// </summary>
        [JsonPropertyName("lastRain")]
        public DateTimeOffset LastRain { get; init; }

        /// <summary>
        /// Unknown value? Probably something to do with Ambient Weathers Databases/Servers?
        /// </summary>`
        [JsonPropertyName("loc")]
        public string? Loc { get; init; }

        /// <summary>
        /// DateTime version of <see cref="EpochMilliseconds"/>.
        /// </summary>
        [JsonPropertyName("date")]
        public DateTimeOffset? UtcDate { get; init; }

// Fields Below Here are not returned by my WS-2902A Weather Station.
// These fields attempt to follow the specification as closely as possible

        /// <summary>
        ///     The direction of the wind gust
        ///     See <see cref="MaxDailyGust" />>.
        /// </summary>
        [JsonPropertyName("windgustdir")]
        public int? WindGustDir { get; init; }

        /// <summary>
        ///     The average wind speed over a 2 minute period in miles per hour.
        /// </summary>
        [JsonPropertyName("windspdmph_avg2m")]
        public double? WindSpeedMph2MinuteAverage { get; init; }

        /// <summary>
        ///     The average wind direction over a 2 minute period.
        /// </summary>
        [JsonPropertyName("winddir_avg2m")]
        public int? WindDirection2MinuteAverage { get; init; }

        /// <summary>
        ///     The average wind speed over a 10 minute period in miles per hour.
        /// </summary>
        [JsonPropertyName("windspdmph_avg10m")]
        public double? WindSpeedMph10MinuteAverage { get; init; }

        /// <summary>
        ///     The average wind direction over a 10 minute period.
        /// </summary>
        [JsonPropertyName("winddir_avg10m")]
        public int? WindDirection10MinuteAverage { get; init; }

        /// <summary>
        ///     A battery indicator for the PM 2.5 Air Quality Sensor
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt_25")]
        public int? PM25AirQualityBatteryLowIndicator { get; init; }

        /// <summary>
        ///     Previous 24 hour rainfall in inches.
        /// </summary>
        [JsonPropertyName("24hourrainin")]
        public double? Previous24HourRainfall { get; init; }

        /// <summary>
        ///     Carbon Dioxide measured in Parts Per Million.
        /// </summary>
        [JsonPropertyName("co2")]
        public double? CO2PartsPerMillion { get; init; }

        /// <summary>
        ///     A battery indicator for the CO2 Sensor.
        ///     <see cref="BatteryLowIndicator1" />
        /// </summary>
        [JsonPropertyName("batt_co2")]
        public int CO2SensorBatteryLowIndicator { get; init; }

        /// <summary>
        ///     Latest Outdoor PM 2.5 Air Quality.
        ///     Measured in micrograms per cubic meter of air (µg/m^3).
        /// </summary>
        [JsonPropertyName("pm25")]
        public double? PM25OutdoorAirQuality { get; init; }

        /// <summary>
        ///     Outdoor PM 2.5 Air Quality, 24 hour average.
        ///     Measured in micrograms per cubic meter of air (µg/m^3).
        /// </summary>
        [JsonPropertyName("pm25_24h")]
        public double? PM25OutdoorAirQuality24HourAverage { get; init; }

        /// <summary>
        ///     Latest Indoor PM 2.5 Air Quality.
        ///     Measured in micrograms per cubic meter of air (µg/m^3).
        /// </summary>
        [JsonPropertyName("pm25_in")]
        public double? PM25IndoorAirQuality { get; init; }

        /// <summary>
        ///     Indoor PM 2.5 Air Quality, 24 hour average.
        ///     Measured in micrograms per cubic meter of air (µg/m^3).
        /// </summary>
        [JsonPropertyName("pm25_in_24h")]
        public double? PM25IndoorAirQuality24HourAverage { get; init; }

        /// <summary>
        ///     Humidity Sensor 1.
        /// </summary>
        [JsonPropertyName("humidity1")]
        public int? HumiditySensor1 { get; init; }

        /// <summary>
        ///     Humidity Sensor 2.
        /// </summary>
        [JsonPropertyName("humidity2")]
        public int? HumiditySensor2 { get; init; }

        /// <summary>
        ///     Humidity Sensor 3.
        /// </summary>
        [JsonPropertyName("humidity3")]
        public int? HumiditySensor3 { get; init; }

        /// <summary>
        ///     Humidity Sensor 4.
        /// </summary>
        [JsonPropertyName("humidity4")]
        public int? HumiditySensor4 { get; init; }

        /// <summary>
        ///     Humidity Sensor 5.
        /// </summary>
        [JsonPropertyName("humidity5")]
        public int? HumiditySensor5 { get; init; }

        /// <summary>
        ///     Humidity Sensor 6.
        /// </summary>
        [JsonPropertyName("humidity6")]
        public int? HumiditySensor6 { get; init; }

        /// <summary>
        ///     Humidity Sensor 7.
        /// </summary>
        [JsonPropertyName("humidity7")]
        public int? HumiditySensor7 { get; init; }

        /// <summary>
        ///     Humidity Sensor 8.
        /// </summary>
        [JsonPropertyName("humidity8")]
        public int? HumiditySensor8 { get; init; }

        /// <summary>
        ///     Humidity Sensor 9.
        /// </summary>
        [JsonPropertyName("humidity9")]
        public int? HumiditySensor9 { get; init; }

        /// <summary>
        ///     Humidity Sensor 10.
        /// </summary>
        [JsonPropertyName("humidity10")]
        public int? HumiditySensor10 { get; init; }

        /// <summary>
        ///     Temperature Sensor 1 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp1f")]
        public double? TemperatureSensor1 { get; init; }

        /// <summary>
        ///     Temperature Sensor 2 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp2f")]
        public double? TemperatureSensor2 { get; init; }

        /// <summary>
        ///     Temperature Sensor 3 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp3f")]
        public double? TemperatureSensor3 { get; init; }

        /// <summary>
        ///     Temperature Sensor 4 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp4f")]
        public double? TemperatureSensor4 { get; init; }

        /// <summary>
        ///     Temperature Sensor 5 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp5f")]
        public double? TemperatureSensor5 { get; init; }

        /// <summary>
        ///     Temperature Sensor 6 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp6f")]
        public double? TemperatureSensor6 { get; init; }

        /// <summary>
        ///     Temperature Sensor 7 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp7f")]
        public double? TemperatureSensor7 { get; init; }

        /// <summary>
        ///     Temperature Sensor 8 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp8f")]
        public double? TemperatureSensor8 { get; init; }

        /// <summary>
        ///     Temperature Sensor 9 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp9f")]
        public double? TemperatureSensor9 { get; init; }

        /// <summary>
        ///     Temperature Sensor 10 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("temp10f")]
        public double? TemperatureSensor10 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 1 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp1f")]
        public double? SoilTemperatureSensor1 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 2 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp2f")]
        public double? SoilTemperatureSensor2 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 3 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp3f")]
        public double? SoilTemperatureSensor3 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 4 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp4f")]
        public double? SoilTemperatureSensor4 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 5 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp5f")]
        public double? SoilTemperatureSensor5 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 6 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp6f")]
        public double? SoilTemperatureSensor6 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 7 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp7f")]
        public double? SoilTemperatureSensor7 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 8 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp8f")]
        public double? SoilTemperatureSensor8 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 9 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp9f")]
        public double? SoilTemperatureSensor9 { get; init; }

        /// <summary>
        ///     Soil Temperature Sensor 10 in Fahrenheit.
        /// </summary>
        [JsonPropertyName("soiltemp10f")]
        public double? SoilTemperatureSensor10 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 1.
        /// </summary>
        [JsonPropertyName("soilhum1")]
        public int? SoilHumiditySensor1 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 2.
        /// </summary>
        [JsonPropertyName("soilhum2")]
        public int? SoilHumiditySensor2 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 3.
        /// </summary>
        [JsonPropertyName("soilhum3")]
        public int? SoilHumiditySensor3 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 4.
        /// </summary>
        [JsonPropertyName("soilhum4")]
        public int? SoilHumiditySensor4 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 5.
        /// </summary>
        [JsonPropertyName("soilhum5")]
        public int? SoilHumiditySensor5 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 6.
        /// </summary>
        [JsonPropertyName("soilhum6")]
        public int? SoilHumiditySensor6 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 7.
        /// </summary>
        [JsonPropertyName("soilhum7")]
        public int? SoilHumiditySensor7 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 8.
        /// </summary>
        [JsonPropertyName("soilhum8")]
        public int? SoilHumiditySensor8 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 9.
        /// </summary>
        [JsonPropertyName("soilhum9")]
        public int? SoilHumiditySensor9 { get; init; }

        /// <summary>
        ///     Soil Humidity Sensor 10.
        /// </summary>
        [JsonPropertyName("soilhum10")]
        public int? SoilHumiditySensor10 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 1.
        ///     A value of 1 represents an 'OK' battery level.
        ///     A value of 0 represents a 'low' battery level.
        ///     For Meteobridge Users: the above value are flipped. See below.
        ///     A value of 0 represents an 'OK' battery level.
        ///     A value of 1 represents a 'low' battery level.
        /// </summary>
        [JsonPropertyName("batt1")]
        public int? BatteryLowIndicator1 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 2
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt2")]
        public int? BatteryLowIndicator2 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 3
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt3")]
        public int? BatteryLowIndicator3 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 4
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt4")]
        public int? BatteryLowIndicator4 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 5
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt5")]
        public int? BatteryLowIndicator5 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 6
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt6")]
        public int? BatteryLowIndicator6 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 7
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt7")]
        public int? BatteryLowIndicator7 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 8
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt8")]
        public int? BatteryLowIndicator8 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 9
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt9")]
        public int? BatteryLowIndicator9 { get; init; }

        /// <summary>
        ///     A battery indicator for sensor 10
        ///     <see cref="BatteryLowIndicator1" />.
        /// </summary>
        [JsonPropertyName("batt10")]
        public int? BatteryLowIndicator10 { get; init; }

        /// <summary>
        ///     Relay Sensor 1 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay1")]
        public int? Relay1 { get; init; }

        /// <summary>
        ///     Relay Sensor 2 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay2")]
        public int? Relay2 { get; init; }

        /// <summary>
        ///     Relay Sensor 3 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay3")]
        public int? Relay3 { get; init; }

        /// <summary>
        ///     Relay Sensor 4 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay4")]
        public int? Relay4 { get; init; }

        /// <summary>
        ///     Relay Sensor 5 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay5")]
        public int? Relay5 { get; init; }

        /// <summary>
        ///     Relay Sensor 6 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay6")]
        public int? Relay6 { get; init; }

        /// <summary>
        ///     Relay Sensor 7 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay7")]
        public int? Relay7 { get; init; }

        /// <summary>
        ///     Relay Sensor 8 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay8")]
        public int? Relay8 { get; init; }

        /// <summary>
        ///     Relay Sensor 9 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay9")]
        public int? Relay9 { get; init; }

        /// <summary>
        ///     Relay Sensor 10 - Value: 0 or 1.
        /// </summary>
        [JsonPropertyName("relay10")]
        public int? Relay10 { get; init; }

        /// <summary>
        ///     IANA TimeZone.
        /// </summary>
        [JsonPropertyName("tz")]
        public string? IANATimeZone { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 1
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike1")]
        public double? FeelsLikeTemperatureFahrenheit1 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 2
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike2")]
        public double? FeelsLikeTemperatureFahrenheit2 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 3
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike3")]
        public double? FeelsLikeTemperatureFahrenheit3 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 4
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike4")]
        public double? FeelsLikeTemperatureFahrenheit4 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 5
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike5")]
        public double? FeelsLikeTemperatureFahrenheit5 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 6
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike6")]
        public double? FeelsLikeTemperatureFahrenheit6 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 7
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike7")]
        public double? FeelsLikeTemperatureFahrenheit7 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 8
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike8")]
        public double? FeelsLikeTemperatureFahrenheit8 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 9
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike9")]
        public double? FeelsLikeTemperatureFahrenheit9 { get; init; }

        /// <summary>
        ///     Feels Like Temperature Sensor 10
        ///     <see cref="OutdoorFeelsLikeTemperatureFahrenheit" />.
        /// </summary>
        [JsonPropertyName("feelsLike10")]
        public double? FeelsLikeTemperatureFahrenheit10 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 1
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint1")]
        public double? DewPointFahrenheit1 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 2
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint2")]
        public double? DewPointFahrenheit2 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 3
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint3")]
        public double? DewPointFahrenheit3 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 4
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint4")]
        public double? DewPointFahrenheit4 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 5
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint5")]
        public double? DewPointFahrenheit5 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 6
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint6")]
        public double? DewPointFahrenheit6 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 7
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint7")]
        public double? DewPointFahrenheit7 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 8
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint8")]
        public double? DewPointFahrenheit8 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 9
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint9")]
        public double? DewPointFahrenheit9 { get; init; }

        /// <summary>
        ///     Dew Point Temperature for Sensor 10
        ///     <see cref="DewPointFahrenheit" />.
        /// </summary>
        [JsonPropertyName("dewPoint10")]
        public double? DewPointFahrenheit10 { get; init; }

        /// <summary>
        ///     Weather Station Mac Address.
        ///     This value is always null when querying the REST API.
        ///     This value is always populated when receiving events from the Websocket (Realtime) API.
        /// </summary>
        [JsonPropertyName("macAddress")]
        public string? MacAddress { get; init; }
    }
}
#pragma warning restore SA1623