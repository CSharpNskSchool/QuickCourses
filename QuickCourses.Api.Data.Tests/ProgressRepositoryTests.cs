using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Authentication;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Data.Repositories;

namespace QuickCourses.Api.Data.Tests
{
    public class ProgressRepositoryTests
    {
        private Settings settings;
        private IProgressRepository progressRepository;
        private Context<CourseProgressData> progressRepositoryContext;

        [SetUp]
        public void Init()
        {
            settings = new Settings(
                connectionString: "mongodb://localhost:27017/",
                database: "Test",
                collectionName: "Progress"
            );

            progressRepository = new ProgressRepository(settings);

            progressRepositoryContext = new Context<CourseProgressData>(settings);
        }

        [TearDown]
        public void Clear()
        {
            progressRepositoryContext.Collection.Database.DropCollection(settings.CollectionName);
        }

        [Test]
        public void GetByLoginTest()
        {
            var user = new UserData { Login = "123", Id = "123" };

            var currentUserProgresses = new[]
            {
                new CourseProgressData {Id = $"{user.Login}{123}", LessonProgresses = new List<LessonProgressData>()},
                new CourseProgressData {Id = $"{user.Login}{124}", LessonProgresses = new List<LessonProgressData>()},
                new CourseProgressData {Id = $"{user.Login}{125}", LessonProgresses = new List<LessonProgressData>()},
            };

            var otherProgresses = new[]
            {
                new CourseProgressData {Id = "someid1", LessonProgresses = new List<LessonProgressData>()},
                new CourseProgressData {Id = "someid2", LessonProgresses = new List<LessonProgressData>()},
                new CourseProgressData {Id = "someid3", LessonProgresses = new List<LessonProgressData>()}
            };

            progressRepositoryContext.Collection.InsertMany(currentUserProgresses);
            progressRepositoryContext.Collection.InsertMany(otherProgresses);

            var result = progressRepository.GetAllByUserAsync(user.Id).Result.ToList();

            var compareLogic = new CompareLogic
            {
                Config =
                {
                    IgnoreCollectionOrder = true,
                    IgnoreObjectTypes = true
                }
            };

            var compareResult = compareLogic.Compare(currentUserProgresses, result);

            Assert.IsTrue(compareResult.AreEqual);
        }
    }
}
