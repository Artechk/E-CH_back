using MongoDB.Driver;
using E_CH_back.Models;

namespace E_CH_back.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MongoSettings:ConnectionString"];
        var databaseName = configuration["MongoSettings:DatabaseName"];
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Doctor> Doctors => _database.GetCollection<Doctor>("doctors");
    public IMongoCollection<Appointment> Appointments => _database.GetCollection<Appointment>("appointments");

}
