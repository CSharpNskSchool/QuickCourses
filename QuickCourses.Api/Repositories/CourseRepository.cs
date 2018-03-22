using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        public Task<IEnumerable<Course>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Task<Course> Get(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
