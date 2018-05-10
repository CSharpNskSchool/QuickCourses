using System;
using MongoDB.Driver;

namespace QuickCourses.Api.Data.Infrastructure
{
    public class DbContext<TValue>
    {
        private readonly IMongoDatabase database;
        private readonly string collectionName;

        public DbContext(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.Database);
            collectionName = settings.CollectionName;
        }

        public IMongoCollection<TValue> Collection => database.GetCollection<TValue>(collectionName);
    }
}
