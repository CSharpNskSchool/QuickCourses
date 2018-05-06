using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IProgressRepository : IRepository<Progress>
    {
        Task<List<Progress>> GetAllByUserAsync(string userId);
        Task<Progress> GetAsync(string userId, string courseId);
        Task<bool> ContainsAsync(string userId, string courseId);
    }
}