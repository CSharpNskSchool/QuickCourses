using MongoDB.Bson;

namespace QuickCourses.Models.Interaction
{
    public class CourseStartOptions
    {
        public int UserId { get; set; }
        public ObjectId CourseId { get; set; }
    }
}
