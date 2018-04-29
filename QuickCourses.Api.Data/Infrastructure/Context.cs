using MongoDB.Driver;

namespace QuickCourses.Api.Data.Infrastructure
{
    public class Context<TValue>
    {
        private readonly IMongoDatabase database;
        private readonly string collectionName;

        public Context(Settings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.Database);
            collectionName = settings.CollectionName;
        }

        public IMongoCollection<TValue> Collection => database.GetCollection<TValue>(collectionName);
    }
}
