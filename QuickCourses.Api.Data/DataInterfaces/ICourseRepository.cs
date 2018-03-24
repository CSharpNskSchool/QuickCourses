using System.Threading.Tasks;
using System.Collections.Generic;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.DataInterfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> Get(int id);
    }
}
