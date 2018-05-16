using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Authentication;
using QuickCourses.Api.Data.Repositories;

namespace QuickCourses.Api.Data.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Settings settings;
        private IUserRepository userRepository;
        private DbContext<UserData> userRepositoryDbContext;

        [SetUp]
        public void Init()
        {
            settings = new Settings(
                connectionString: "mongodb://localhost:27017/",
                database: "Test",
                collectionName: "Users"
            );

            userRepository = new UserRepository(settings);

            userRepositoryDbContext = new DbContext<UserData>(settings);
        }

        [TearDown]
        public void Clear()
        {
            userRepositoryDbContext.Collection.Database.DropCollection(settings.CollectionName);
        }

        [Test]
        public void GetByLoginTest()
        {
            var user = new UserData {Login = "123", Id = "123"};

            userRepositoryDbContext.Collection.InsertOne(user);

            var result = userRepository.GetByLoginAsync(user.Login).Result;

            var compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(user, result);

            Assert.IsTrue(compareResult.AreEqual);
        }
    }
}
