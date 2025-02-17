using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using WeatherScheduledNotification.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WeatherLambdaFunction;

public class Function
{
    private WeatherService _weatherService;
    private readonly string queueUrl;
    private readonly AmazonSQSClient _sqsClient;


    public Function()
    {
        _weatherService = new WeatherService();
        queueUrl = Environment.GetEnvironmentVariable("QUEUE_URL");
        _sqsClient = new AmazonSQSClient();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(string input, ILambdaContext context)
    {
        var city = "Westminster, CO";
        var weatherDetails = _weatherService.GetWeather(city);

        var message = new
        {
            id = Guid.NewGuid().ToString(),
            timestamp = DateTime.UtcNow.ToString("o"),
            staus = "success",
            data = new
            {
                message = "weather details",
                condition = weatherDetails.CurrentWeather.Condition.Text,
                city,
                tempf = weatherDetails.CurrentWeather.TemperatureFahrenheit
            }
        };
        var messageBody = JsonSerializer.Serialize(message);
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody
        };

        await _sqsClient.SendMessageAsync(sendMessageRequest);
    }
}