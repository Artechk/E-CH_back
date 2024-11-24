using MongoDB.Driver;
using E_CH_back.Models;

namespace E_CH_back.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        // Получаем строку подключения из секции MongoSettings
        var connectionString = configuration["MongoSettings:ConnectionString"];
        Console.WriteLine($"Connection String: {connectionString}");
        var databaseName = configuration["MongoSettings:DatabaseName"];

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is not configured.");

        if (string.IsNullOrEmpty(databaseName))
            throw new ArgumentNullException(nameof(databaseName), "MongoDB database name is not configured.");

        // Создаем настройки клиента MongoDB
        var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
        clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

        // Инициализируем базу данных
        var client = new MongoClient(clientSettings);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Doctor> Doctors => _database.GetCollection<Doctor>("doctors");
    public IMongoCollection<Appointment> Appointments => _database.GetCollection<Appointment>("appointments");
    public IMongoCollection<Payment> Payments => _database.GetCollection<Payment>("payments");
}

