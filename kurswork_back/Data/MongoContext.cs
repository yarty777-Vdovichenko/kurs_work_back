using kurswork_back.Models;
using MongoDB.Driver;

namespace kurswork_back.Data
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IConfiguration configuration)
        {
            var connectionString =
                configuration["MongoDB:ConnectionString"];

            var client = new MongoClient(connectionString);

            _database = client.GetDatabase(
                configuration["MongoDB:DatabaseName"]
            );
        }

        public IMongoCollection<User> Users =>
            _database.GetCollection<User>("users");

        public IMongoCollection<Tarif> Tarifs =>
            _database.GetCollection<Tarif>("tarifs");


    }
}