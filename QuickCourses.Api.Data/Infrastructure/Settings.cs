namespace QuickCourses.Api.Data.Infrastructure
{
    public class Settings
    {
        public Settings(string collectionName, string connectionString, string database)
        {
            CollectionName = collectionName;
            ConnectionString = connectionString;
            Database = database;
        }
        
        public string ConnectionString { get; }
        public string Database { get; }
        public string CollectionName { get; }
    }
}
