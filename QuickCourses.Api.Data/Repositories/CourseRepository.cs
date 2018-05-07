using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;

namespace QuickCourses.Api.Data.Repositories
{
    public class CourseRepository : RepositoryBase<CourseData>
    {
        public CourseRepository(Settings settings)
            : base(settings)
        {
        }

        public override async Task<List<CourseData>> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            result.ForEach(course => course.SetUpLinks());
            return result;
        }

        public override async Task<CourseData> GetAsync(string id)
        {
            var result = await base.GetAsync(id);
            result?.SetUpLinks();
            return result;
        }
    }
}
