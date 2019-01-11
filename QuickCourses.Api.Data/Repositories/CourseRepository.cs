using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;

namespace QuickCourses.Api.Data.Repositories
{
    public class CourseRepository : RepositoryBase<CourseData>, ICourseRepository
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

        public string GenerateNewId(string authorId)
        {
            var id = GenerateNewId();
            return $"{authorId}{id}";
        }
    }
}
