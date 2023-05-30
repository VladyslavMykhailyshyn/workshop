using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Workshop.Entities;

namespace Workshop.Repositories;

public interface IUsersRepository
{
    public void AddUser(User user);
    public User GetUser(string id);
    public User GetUserByUsername(string username);
    public void UpdateUser(User user);
    public void DeleteUser(string id);
    public IEnumerable<User> GetAllUsers();
}


public class UsersRepository: IUsersRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>("users");
    }

    public void AddUser(User user)
    {
        _usersCollection.InsertOne(user);
    }

    public User GetUser(string id)
    {
        return _usersCollection.FindSync(u => u.Id == id).FirstOrDefault();
    }

    public User GetUserByUsername(string username)
    {
        return _usersCollection.FindSync(u => u.Username == username).FirstOrDefault();
    }

    public void UpdateUser(User user)
    {
        _usersCollection.ReplaceOne(u => u.Id == user.Id, user);
    }

    public void DeleteUser(string id)
    {
        _usersCollection.DeleteOne(u => u.Id == id);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _usersCollection.FindSync(_ => true).ToList();
    }
}