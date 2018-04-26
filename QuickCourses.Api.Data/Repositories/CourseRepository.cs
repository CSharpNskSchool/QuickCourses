using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Extensions;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Data.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly Context<Course> context;

        public CourseRepository(Settings settings)
        {
            context = new Context<Course>(settings, collectionName: "Courses");
        }

        public async Task<IEnumerable<Course>> GetAll()
        {
            var courses = await context.Collection.Find(_ => true).ToListAsync();
            var result = courses.Select(x => x.SetUpLinks());
            return result;
        }

        public async Task<Course> Get(string id)
        {
            var result = await context.Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            result.SetUpLinks();
            return result;
        }

        public async Task Insert(Course course)
        {
            course.Id = ObjectId.GenerateNewId().ToString();;
            await context.Collection.InsertOneAsync(course);
        }

        public async Task Delete(string id)
        {
            await context.Collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
