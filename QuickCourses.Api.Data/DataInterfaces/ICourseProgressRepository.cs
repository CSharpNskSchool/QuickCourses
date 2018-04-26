using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface ICourseProgressRepository
    {
        Task<IEnumerable<CourseProgress>> GetAll(string userId);
        Task<CourseProgress> Get(string userId, string courseId);
        Task<bool> Contains(string userId, string courseId);
        Task Update(CourseProgress courseProgress);
        Task Insert(CourseProgress courseProgress);
        Task Delete(string userId, string courseId);
    }
}
