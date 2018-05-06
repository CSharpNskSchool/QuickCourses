using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Models.Authentication;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.Tests
{
    public class ProgressRepositoryTests
    {
        private Settings settings;
        private IProgressRepository progressRepository;
        private Context<CourseProgress> progressRepositoryContext;

        [SetUp]
        public void Init()
        {
            settings = new Settings(
                connectionString: "mongodb://localhost:27017/",
                database: "Test",
                collectionName: "Progress"
            );

            progressRepository = new ProgressRepository(settings);

            progressRepositoryContext = new Context<CourseProgress>(settings);
        }

        [TearDown]
        public void Clear()
        {
            progressRepositoryContext.Collection.Database.DropCollection(settings.CollectionName);
        }

        [Test]
        public void GetByLoginTest()
        {
            var user = new User { Login = "123", Id = "123" };

            var currentUserProgresses = new[]
            {
                new CourseProgress {Id = $"{user.Login}{123}", LessonProgresses = new List<LessonProgress>()},
                new CourseProgress {Id = $"{user.Login}{124}", LessonProgresses = new List<LessonProgress>()},
                new CourseProgress {Id = $"{user.Login}{125}", LessonProgresses = new List<LessonProgress>()},
            };

            var otherProgresses = new[]
            {
                new CourseProgress {Id = "someid1", LessonProgresses = new List<LessonProgress>()},
                new CourseProgress {Id = "someid2", LessonProgresses = new List<LessonProgress>()},
                new CourseProgress {Id = "someid3", LessonProgresses = new List<LessonProgress>()}
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
