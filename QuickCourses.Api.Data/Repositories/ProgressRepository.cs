using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Extensions;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.Repositories
{
    public class ProgressRepository : RepositoryBase<CourseProgress>, IProgressRepository
    {
        public ProgressRepository(Settings settings) 
            : base(settings)
        {
        }

        public async Task<List<CourseProgress>> GetAllByUserAsync(string userId)
        {
            var result = await Context.Collection.Find(progress => progress.Id.StartsWith(userId)).ToListAsync();
            result.ForEach(progress => progress.SetUpLinks());
            return result;
        }

        public override async Task<List<CourseProgress>> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            result.ForEach(progress => progress.SetUpLinks());
            return result;
        }

        public override async Task<CourseProgress> GetAsync(string id)
        {
            var result = await base.GetAsync(id);
            result.SetUpLinks();
            return result;
        }
    }
}