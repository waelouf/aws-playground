using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Scrapper.Lambda;

public class Function
{
    private IUsersRepository _usersRepository;
    
    public Function(): this(null)
    {
    }

    public Function(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository ?? new UsersRepository();
    }
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(string input, ILambdaContext context)
    {
        //var dynamoContext = new DynamoDBContext(new AmazonDynamoDBClient());
        await _usersRepository.SaveAsync(new User(Guid.NewGuid().ToString(),input));
        //await dynamoContext.SaveAsync();
    }
}