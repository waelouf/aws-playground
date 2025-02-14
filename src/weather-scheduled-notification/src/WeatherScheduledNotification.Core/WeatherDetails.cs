namespace WeatherScheduledNotification.Core;

using Newtonsoft.Json;

    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lon")]
        public double Longitude { get; set; }

        [JsonProperty("tz_id")]
        public string TimeZoneId { get; set; }

        [JsonProperty("localtime_epoch")]
        public long LocalTimeEpoch { get; set; }

        [JsonProperty("localtime")]
        public string LocalTime { get; set; }
    }

    public class Condition
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

    public class CurrentWeather
    {
        [JsonProperty("last_updated_epoch")]
        public long LastUpdatedEpoch { get; set; }

        [JsonProperty("last_updated")]
        public string LastUpdated { get; set; }

        [JsonProperty("temp_c")]
        public double TemperatureCelsius { get; set; }

        [JsonProperty("temp_f")]
        public double TemperatureFahrenheit { get; set; }

        [JsonProperty("is_day")]
        public int IsDay { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("wind_mph")]
        public double WindSpeedMph { get; set; }

        [JsonProperty("wind_kph")]
        public double WindSpeedKph { get; set; }

        [JsonProperty("wind_degree")]
        public int WindDegree { get; set; }

        [JsonProperty("wind_dir")]
        public string WindDirection { get; set; }

        [JsonProperty("pressure_mb")]
        public double PressureMb { get; set; }

        [JsonProperty("pressure_in")]
        public double PressureIn { get; set; }

        [JsonProperty("precip_mm")]
        public double PrecipitationMm { get; set; }

        [JsonProperty("precip_in")]
        public double PrecipitationIn { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }

        [JsonProperty("cloud")]
        public int Cloud { get; set; }

        [JsonProperty("feelslike_c")]
        public double FeelsLikeCelsius { get; set; }

        [JsonProperty("feelslike_f")]
        public double FeelsLikeFahrenheit { get; set; }

        [JsonProperty("vis_km")]
        public double VisibilityKm { get; set; }

        [JsonProperty("vis_miles")]
        public double VisibilityMiles { get; set; }

        [JsonProperty("uv")]
        public double UV { get; set; }

        [JsonProperty("gust_mph")]
        public double GustSpeedMph { get; set; }

        [JsonProperty("gust_kph")]
        public double GustSpeedKph { get; set; }
    }

    public class WeatherDetails
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public CurrentWeather CurrentWeather { get; set; }
    }
