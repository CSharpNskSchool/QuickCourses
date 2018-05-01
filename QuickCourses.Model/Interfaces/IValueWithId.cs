using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Interfaces
{
    public interface IValueWithId
    {
        [BsonId]
        string Id { get; set; }
    }
}