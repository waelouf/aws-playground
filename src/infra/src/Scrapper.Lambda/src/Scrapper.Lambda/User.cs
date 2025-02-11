namespace Scrapper.Lambda;

public class User
{
    public string Id { get; set; }

    public string Username { get; set; }
    
    public User()
    {
        
    }

    public User(string id, string username)
    {
        Id = id;
        Username = username;
    }
}