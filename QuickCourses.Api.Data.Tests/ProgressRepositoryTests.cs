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
        private Context<Progress> context;

        [SetUp]
        public void Init()
        {
            settings = new Settings(
                connectionString: "mongodb://localhost:27017/",
                database: "Test",
                collectionName: "Progress"
            );

            progressRepository = new ProgressRepository(settings);

            context = new Context<Progress>(settings);
        }

        [TearDown]
        public void Clear()
        {
            context.Collection.Database.DropCollection(settings.CollectionName);
        }

        [Test]
        public void GetByLoginTest()
        {
            var user = new User { Login = "123", Id = "123" };

            var currentUserProgresses = new[]
            {
                new Progress {Id = $"{user.Login}{123}", LessonProgresses = new List<LessonProgress>()},
                new Progress {Id = $"{user.Login}{124}", LessonProgresses = new List<LessonProgress>()},
                new Progress {Id = $"{user.Login}{125}", LessonProgresses = new List<LessonProgress>()},
            };

            var otherProgresses = new[]
            {
                new Progress {Id = "someid1", LessonProgresses = new List<LessonProgress>()},
                new Progress {Id = "someid2", LessonProgresses = new List<LessonProgress>()},
                new Progress {Id = "someid3", LessonProgresses = new List<LessonProgress>()}
            };

            context.Collection.InsertMany(currentUserProgresses);
            context.Collection.InsertMany(otherProgresses);

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
