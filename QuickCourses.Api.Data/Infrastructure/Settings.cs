namespace QuickCourses.Api.Data.Infrastructure
{
    public class Settings
    {
        public Settings()
        {
        }

        public Settings(string connectionString, string database, string collectionName)
        {
            ConnectionString = connectionString;
            Database = database;
            CollectionName = collectionName;
        }
        
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string CollectionName { get; set; }
    }
}
