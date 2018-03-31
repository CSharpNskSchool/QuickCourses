using MongoDB.Bson;

namespace QuickCourses.Models.Interaction
{
    public class CourseStartOptions
    {
        public int UserId { get; set; }
        public string CourseId { get; set; }
    }
}
