using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.Repositories
{
    public class CourseProgressRepository : ICourseProgressRepository
    {
        private readonly Context<CourseProgress> context;

        public CourseProgressRepository(Settings settings)
        {
            context = new Context<CourseProgress>(settings, collectionName: "Progress");
        }

        public async Task<CourseProgress> Get(string userId, string courseId)
        {
            var filter = Builders<CourseProgress>.Filter.And(
                    Builders<CourseProgress>.Filter.Eq(x => x.CourceId, courseId),
                    Builders<CourseProgress>.Filter.Eq(x => x.UserId, userId)
                );

            var result = await context.Collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> Contains(string userId, string courseId)
        {
            var progress = await Get(userId, courseId);

            return progress != null;
        }

        public async Task<IEnumerable<CourseProgress>> GetAll(string userId)
        {
            var result = await context.Collection.Find(x => x.UserId == userId).ToListAsync();
            return result;
        }

        public async Task Update(CourseProgress courseProgress)
        {
            await context.Collection.ReplaceOneAsync(x => x.CourceId == courseProgress.CourceId, courseProgress);
        }

        public async Task Insert(CourseProgress courseProgress)
        {
            courseProgress.Id = ObjectId.GenerateNewId().ToString();;
            await context.Collection.InsertOneAsync(courseProgress);
        }

        public async Task Delete(string userId, string courseId)
        {
            var filter = Builders<CourseProgress>.Filter.And(
                Builders<CourseProgress>.Filter.Eq(x => x.CourceId, courseId),
                Builders<CourseProgress>.Filter.Eq(x => x.UserId, userId)
            );

            await context.Collection.DeleteOneAsync(filter);
        }
    }
}
