using System.Formats.Asn1;
using Newtonsoft.Json;
using RestSharp;

namespace WeatherScheduledNotification.Core;

public class WeatherService : IWeatherService
{
    private string apiKey = "";


    public WeatherService()
    {
         apiKey = Environment.GetEnvironmentVariable("WeatherApi");
        if (apiKey is null) throw new ArgumentNullException("WeatherApi is null");
    }

    public WeatherService(string key)
    {
        apiKey = key;
    }

    public WeatherDetails GetWeather(string city)
    {
        var options = new RestClientOptions("http://api.weatherapi.com")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest($"/v1/current.json?key={apiKey}&q={city}&aqi=no", Method.Get);
        RestResponse response = client.ExecuteAsync(request).Result;
        WeatherDetails weather = JsonConvert.DeserializeObject<WeatherDetails>(response.Content);

        return weather;
    }
}