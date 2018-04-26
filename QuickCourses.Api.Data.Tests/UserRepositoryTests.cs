using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using NUnit.Framework;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Data.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private UserRepository userRepository;
        private User user;
        private Context<User> context;
        private string collectionName;

        [SetUp]
        public void Init()
        {
            var settings = new Settings
            {
                ConnectionString = "mongodb://localhost:27017/",
                Database = "Test" 
            };

            collectionName = "Users";

            userRepository = new UserRepository(settings);
            user = new User
            {
                Login = "123",
                Password = "123",
                Name = "123"
            };

            context = new Context<User>(settings, collectionName);
        }

        [TearDown]
        public void Cleare()
        {
            context.Collection.Database.DropCollection(collectionName);
        }

        private class UserComparer : IComparer
        {
            public int Compare(User x, User y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (x.Login == y.Login && x.Password == y.Password && x.Name == y.Name)
                {
                    return 0;
                }

                return 1;
            }

            public int Compare(object x, object y)
            {
                if (!(x is User lft) || !(y is User rht))
                {
                   return 1;
                }

                return Compare(lft, rht);
            }
        }

        [Test]
        public void InsertTest()
        {
            userRepository.Insert(user).Wait();

            var result = context.Collection.Find(x => x.Login == user.Login).ToList();
            var expectedResult = new[] {user};

            CollectionAssert.AreEqual(expectedResult, result, new UserComparer());
        }

        [Test]
        public void DeleteTest()
        {
            context.Collection.InsertOne(user);

            userRepository.Delete(user.Login).Wait();

            var result = context.Collection.Find(x => x.Login == user.Login).ToList();
            var expectedResult = new User[0];

            CollectionAssert.AreEqual(expectedResult, result, new UserComparer());
        }

        [Test]
        public void 
    }
}
