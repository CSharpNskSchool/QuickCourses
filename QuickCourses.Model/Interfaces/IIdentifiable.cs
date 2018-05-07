using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Api.Data.Models.Interfaces
{
    public interface IIdentifiable
    {
        [BsonId]
        string Id { get; set; }
    }
}