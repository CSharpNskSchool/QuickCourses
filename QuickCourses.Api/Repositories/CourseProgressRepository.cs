using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;
using QuickCourses.Model.Progress;

namespace QuickCourses.Api.Repositories
{
    public class CourseProgressRepository : ICourseProgressRepository
    {
        public Task<CourseProgress> Get(int userId, int courseId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<CourseProgress>> GetAll(int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task Update(CourseProgress courseProgress)
        {
            throw new System.NotImplementedException();
        }

        public Task Insert(CourseProgress courseProgress)
        {
            throw new System.NotImplementedException();
        }
    }
}
