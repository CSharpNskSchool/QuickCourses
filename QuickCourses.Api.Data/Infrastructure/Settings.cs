namespace QuickCourses.Api.Data.Infrastructure
{
    public class Settings
    {
        public Settings(string connectionString, string database, string collectionName)
        {
            ConnectionString = connectionString;
            Database = database;
            CollectionName = collectionName;
        }
        
        public string ConnectionString { get; }
        public string Database { get; }
        public string CollectionName { get; }
    }
}
