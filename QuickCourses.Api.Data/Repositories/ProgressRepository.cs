using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Progress;

namespace QuickCourses.Api.Data.Repositories
{
    public class ProgressRepository : RepositoryBase<CourseProgressData>, IProgressRepository
    {
        public ProgressRepository(Settings settings) 
            : base(settings)
        {
        }

        public async Task<List<CourseProgressData>> GetAllByUserAsync(string userId)
        {
            if (userId == null)
            {
                throw new System.ArgumentNullException(nameof(userId));
            }

            var result = await Context.Collection.Find(progress => progress.Id.StartsWith(userId)).ToListAsync();
            result.ForEach(progress => progress.SetUpLinks());
            return result;
        }

        public Task<CourseProgressData> GetAsync(string userId, string courseId)
        {
            if (userId == null)
            {
                throw new System.ArgumentNullException(nameof(userId));
            }

            if (courseId == null)
            {
                throw new System.ArgumentNullException(nameof(courseId));
            }

            return GetAsync($"{userId}{courseId}");
        }

        public Task<bool> ContainsAsync(string userId, string courseId)
        {
            if (userId == null)
            {
                throw new System.ArgumentNullException(nameof(userId));
            }

            if (courseId == null)
            {
                throw new System.ArgumentNullException(nameof(courseId));
            }

            return ContainsAsync($"{userId}{courseId}");
        }

        public override async Task<List<CourseProgressData>> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            result.ForEach(progress => progress.SetUpLinks());
            return result;
        }

        public override async Task<CourseProgressData> GetAsync(string id)
        {
            if (id == null)
            {
                throw new System.ArgumentNullException(nameof(id));
            }

            var result = await base.GetAsync(id);
            result?.SetUpLinks();
            return result;
        }

        public override async Task<string> InsertAsync(CourseProgressData value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException(nameof(value));
            }

            var id = $"{value.UserId}{value.CourceId}";
            value.Id = id;
            await Context.Collection.InsertOneAsync(value);
            return id;
        }
    }
}