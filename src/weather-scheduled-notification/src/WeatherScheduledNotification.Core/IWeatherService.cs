namespace WeatherScheduledNotification.Core;

public interface IWeatherService
{
    WeatherDetails GetWeather(string city);
}