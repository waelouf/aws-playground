using Amazon.Lambda.Core;
using WeatherScheduledNotification.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WeatherLambdaFunction;

public class Function
{
    private WeatherService _weatherService;

    public Function()
    {
        _weatherService = new WeatherService();
    }
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string FunctionHandler(string input, ILambdaContext context)
    {
        var city = "Westminster, CO";
        var weatherDetails = _weatherService.GetWeather(city);
        
        return input.ToUpper();
    }
}