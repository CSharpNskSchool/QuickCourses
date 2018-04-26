using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> Get(string id);
        Task Insert(Course course);
        Task Delete(string id);
    }
}
