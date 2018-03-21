using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        public Task<IEnumerable<Course>> GetAllCourses()
        {
            throw new System.NotImplementedException();
        }

        public Task<Course> GetCourse(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
