using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace Scrapper.Lambda;

public class UsersRepository : IUsersRepository
{
    private DynamoDBContext _context;
    public UsersRepository()
    {
        _context = new DynamoDBContext(new AmazonDynamoDBClient());
    }

    public async Task SaveAsync(User user)
    {
        await _context.SaveAsync(user);
    }
}