using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.Data.Models.Progress;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IProgressRepository : IRepository<CourseProgressData>
    {
        Task<List<CourseProgressData>> GetAllByUserAsync(string userId);
        Task<CourseProgressData> GetAsync(string userId, string courseId);
        Task<bool> ContainsAsync(string userId, string courseId);
    }
}