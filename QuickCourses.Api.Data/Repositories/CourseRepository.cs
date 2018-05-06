using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Extensions;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Data.Repositories
{
    public class CourseRepository : RepositoryBase<Course>
    {
        public CourseRepository(Settings settings)
            : base(settings)
        {
        }

        public override async Task<List<Course>> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            result.ForEach(course => course.SetUpLinks());
            return result;
        }

        public override async Task<Course> GetAsync(string id)
        {
            var result = await base.GetAsync(id);
            result?.SetUpLinks();
            return result;
        }
    }
}
