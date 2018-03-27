using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> Get(int id);
    }
}
