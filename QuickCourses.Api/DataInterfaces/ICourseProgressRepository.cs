using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Model.Primitives;
using QuickCourses.Model.Progress;

namespace QuickCourses.Api.DataInterfaces
{
    public interface ICourseProgressRepository
    {
        Task<CourseProgress> Get(int userId, int courseId);
        Task<IEnumerable<CourseProgress>> GetAll(int userId);
        Task Update(CourseProgress courseProgress);
        Task Insert(CourseProgress courseProgress);
    }
}
