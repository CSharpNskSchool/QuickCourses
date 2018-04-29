using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Interfaces
{
    public interface IValueWithId
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
    }
}