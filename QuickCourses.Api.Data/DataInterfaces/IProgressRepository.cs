using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IProgressRepository : IRepository<CourseProgress>
    {
        Task<List<CourseProgress>> GetAllByUserAsync(string userId);
        Task<CourseProgress> GetAsync(string userId, string courseId);
        Task<bool> ContainsAsync(string userId, string courseId);
    }
}