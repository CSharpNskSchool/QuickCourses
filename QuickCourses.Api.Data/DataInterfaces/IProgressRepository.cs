using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IProgressRepository : IRepository<CourseProgress>
    {
        Task<IEnumerable<CourseProgress>> GetAllByUser(string userId);
    }
}