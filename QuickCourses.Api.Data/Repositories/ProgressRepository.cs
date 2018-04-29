using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.Repositories
{
    public class ProgressRepository : Repository<CourseProgress>, IProgressRepository
    {
        public ProgressRepository(Settings settings) 
            : base(settings)
        {
        }

        public async Task<IEnumerable<CourseProgress>> GetAllByUser(string userId)
        {
            var result = await context.Collection.Find(progress => progress.Id.StartsWith(userId)).ToListAsync();
            return result;
        }
    }
}