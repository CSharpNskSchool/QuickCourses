using QuickCourses.Api.Data.Models.Primitives;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface ICourseRepository : IRepository<CourseData>
    {
        string GenerateNewId(string authorId);
    }
}
