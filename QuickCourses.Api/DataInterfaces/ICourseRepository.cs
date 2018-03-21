using System.Threading.Tasks;
using System.Collections.Generic;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.DataInterfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCourses();
        Task<Course> GetCourse(int id);
    }
}
