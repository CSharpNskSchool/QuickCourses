using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Interfaces
{
    public interface IIdentifiable
    {
        [BsonId]
        string Id { get; set; }
    }
}