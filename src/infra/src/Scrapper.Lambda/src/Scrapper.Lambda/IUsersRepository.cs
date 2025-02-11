namespace Scrapper.Lambda;

public interface IUsersRepository
{
    Task SaveAsync(User user);
}