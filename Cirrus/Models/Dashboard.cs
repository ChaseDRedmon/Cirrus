using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cirrus.Models;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record Alerts(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("freq")] string Freq
    );

    public record Baromabsin(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Baromrelin(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Coords(
        [property: JsonPropertyName("coords")] Coords Coords,
        [property: JsonPropertyName("location")] string Location,
        [property: JsonPropertyName("elevation")] double Elevation,
        [property: JsonPropertyName("geo")] Geo Geo,
        [property: JsonPropertyName("lon")] double Lon,
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("address")] string Address
    );

    public record Dailyrainin(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Dashboard(
        [property: JsonPropertyName("minimized")] bool Minimized
    );

    public record Datum(
        [property: JsonPropertyName("_id")] string Id,
        [property: JsonPropertyName("macAddress")] string MacAddress,
        [property: JsonPropertyName("lastData")] LastData LastData,
        [property: JsonPropertyName("settings")] Settings Settings,
        [property: JsonPropertyName("info")] Info Info,
        [property: JsonPropertyName("tz")] Tz Tz,
        [property: JsonPropertyName("unitSettings")] UnitSettings UnitSettings
    );

    public record DewPoint(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record DewPoint1(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record DewPointin(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Discreets(
        [property: JsonPropertyName("humidity1")] IReadOnlyList<int> Humidity1
    );

    public record Eventrainin(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record FeelsLike(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record FeelsLike1(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record FeelsLikein(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Geo(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("coordinates")] IReadOnlyList<double> Coordinates
    );

    public record Hl(
        [property: JsonPropertyName("dateutc")] long Dateutc,
        [property: JsonPropertyName("dewPoint")] DewPoint DewPoint,
        [property: JsonPropertyName("feelsLike")] FeelsLike FeelsLike,
        [property: JsonPropertyName("windspeedmph")] Windspeedmph Windspeedmph,
        [property: JsonPropertyName("windgustmph")] Windgustmph Windgustmph,
        [property: JsonPropertyName("maxdailygust")] Maxdailygust Maxdailygust,
        [property: JsonPropertyName("humidity")] Humidity Humidity,
        [property: JsonPropertyName("humidity1")] Humidity1 Humidity1,
        [property: JsonPropertyName("humidityin")] Humidityin Humidityin,
        [property: JsonPropertyName("tempf")] Tempf Tempf,
        [property: JsonPropertyName("temp1f")] Temp1f Temp1f,
        [property: JsonPropertyName("feelsLike1")] FeelsLike1 FeelsLike1,
        [property: JsonPropertyName("feelsLikein")] FeelsLikein FeelsLikein,
        [property: JsonPropertyName("dewPoint1")] DewPoint1 DewPoint1,
        [property: JsonPropertyName("dewPointin")] DewPointin DewPointin,
        [property: JsonPropertyName("tempinf")] Tempinf Tempinf,
        [property: JsonPropertyName("hourlyrainin")] Hourlyrainin Hourlyrainin,
        [property: JsonPropertyName("dailyrainin")] Dailyrainin Dailyrainin,
        [property: JsonPropertyName("weeklyrainin")] Weeklyrainin Weeklyrainin,
        [property: JsonPropertyName("monthlyrainin")] Monthlyrainin Monthlyrainin,
        [property: JsonPropertyName("eventrainin")] Eventrainin Eventrainin,
        [property: JsonPropertyName("totalrainin")] Totalrainin Totalrainin,
        [property: JsonPropertyName("baromrelin")] Baromrelin Baromrelin,
        [property: JsonPropertyName("baromabsin")] Baromabsin Baromabsin,
        [property: JsonPropertyName("uv")] Uv Uv,
        [property: JsonPropertyName("solarradiation")] Solarradiation Solarradiation,
        [property: JsonPropertyName("winddir")] Winddir Winddir
    );

    public record Hourlyrainin(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Humidity(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Humidity1(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Humidityin(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Info(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("coords")] Coords Coords,
        [property: JsonPropertyName("indoor")] bool Indoor,
        [property: JsonPropertyName("slug")] string Slug
    );

    public record LastData(
        [property: JsonPropertyName("stationtype")] string Stationtype,
        [property: JsonPropertyName("dateutc")] long Dateutc,
        [property: JsonPropertyName("baromrelin")] double Baromrelin,
        [property: JsonPropertyName("baromabsin")] double Baromabsin,
        [property: JsonPropertyName("tempf")] double Tempf,
        [property: JsonPropertyName("humidity")] int Humidity,
        [property: JsonPropertyName("winddir")] int Winddir,
        [property: JsonPropertyName("windspeedmph")] double Windspeedmph,
        [property: JsonPropertyName("windgustmph")] double Windgustmph,
        [property: JsonPropertyName("maxdailygust")] double Maxdailygust,
        [property: JsonPropertyName("hourlyrainin")] int Hourlyrainin,
        [property: JsonPropertyName("eventrainin")] int Eventrainin,
        [property: JsonPropertyName("dailyrainin")] int Dailyrainin,
        [property: JsonPropertyName("weeklyrainin")] int Weeklyrainin,
        [property: JsonPropertyName("monthlyrainin")] double Monthlyrainin,
        [property: JsonPropertyName("totalrainin")] double Totalrainin,
        [property: JsonPropertyName("solarradiation")] double Solarradiation,
        [property: JsonPropertyName("uv")] int Uv,
        [property: JsonPropertyName("batt_co2")] int BattCo2,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("created_at")] long CreatedAt,
        [property: JsonPropertyName("feelsLike")] double FeelsLike,
        [property: JsonPropertyName("dewPoint")] double DewPoint,
        [property: JsonPropertyName("dateutc5")] long Dateutc5,
        [property: JsonPropertyName("lastRain")] long LastRain,
        [property: JsonPropertyName("discreets")] Discreets Discreets,
        [property: JsonPropertyName("tz")] string Tz,
        [property: JsonPropertyName("hl")] Hl Hl
    );

    public record Maxdailygust(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Monthlyrainin(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Recent(
        [property: JsonPropertyName("_id")] string Id,
        [property: JsonPropertyName("info")] Info Info,
        [property: JsonPropertyName("macAddress")] string MacAddress
    );

    public record Root(
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("skip")] int Skip,
        [property: JsonPropertyName("data")] IReadOnlyList<Datum> Data
    );

    public record Settings(
        [property: JsonPropertyName("wind")] Wind Wind
    );

    public record Solarradiation(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Temp1f(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Tempf(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Tempinf(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Theme(
        [property: JsonPropertyName("theme")] string Theme
    );

    public record Totalrainin(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] double L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Tz(
        [property: JsonPropertyName("name")] string Name
    );

    public record UnitSettings(
        [property: JsonPropertyName("recent")] IReadOnlyList<Recent> Recent,
        [property: JsonPropertyName("dashboard")] Dashboard Dashboard,
        [property: JsonPropertyName("alerts")] Alerts Alerts,
        [property: JsonPropertyName("theme")] Theme Theme
    );

    public record Uv(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Weeklyrainin(
        [property: JsonPropertyName("h")] int H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] int S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Wind(
        [property: JsonPropertyName("dashboard")] Dashboard Dashboard
    );

    public record Winddir(
        [property: JsonPropertyName("WNW")] int WNW,
        [property: JsonPropertyName("W")] int W,
        [property: JsonPropertyName("NNW")] int NNW,
        [property: JsonPropertyName("N")] int N,
        [property: JsonPropertyName("ENE")] int ENE,
        [property: JsonPropertyName("NNE")] int NNE,
        [property: JsonPropertyName("NE")] int NE,
        [property: JsonPropertyName("NW")] int NW,
        [property: JsonPropertyName("E")] int E,
        [property: JsonPropertyName("ESE")] int ESE,
        [property: JsonPropertyName("SE")] int SE,
        [property: JsonPropertyName("SSE")] int SSE,
        [property: JsonPropertyName("SSW")] int SSW,
        [property: JsonPropertyName("SW")] int SW,
        [property: JsonPropertyName("WSW")] int WSW
    );

    public record Windgustmph(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );

    public record Windspeedmph(
        [property: JsonPropertyName("h")] double H,
        [property: JsonPropertyName("l")] int L,
        [property: JsonPropertyName("c")] int C,
        [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("ht")] long Ht,
        [property: JsonPropertyName("lt")] long Lt
    );