using System.Collections;
using NUnit.Framework;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Models.Authentication;
using QuickCourses.Models.Interfaces;

namespace QuickCourses.Api.Data.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class UserRepositoryTests
    {
        private class Value : IValueWithId
        {
            private static int count;
            
            public Value()
            {
                Id = (++count).ToString();
            }
            
            public string Id { get; set; }
        }
        
        private Repository<Value> userRepository;
        private Context<Value> context;
        
        

        [SetUp]
        public void Init()
        {
            var settings = new Settings(
                connectionString: "mongodb://localhost:27017/",
                database: "Test",
                collectionName: "Value"
            );

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
            public int Compare(object x, object y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }
                
                if (!(x is User lft) || !(y is User rht))
                {
                   return 1;
                }

                if (lft.Login == rht.Login && lft.Password == rht.Password && lft.Name == rht.Name)
                {
                    return 0;
                }

                return 1;
            }
        }

        [Test]
        public void InsertTest()
        {
            Assert.DoesNotThrow(() => userRepository.Insert(user).Wait());
        }

        [Test]
        public void DeleteTest()
        {
            context.Collection.InsertOne(user);

            var result = userRepository.Delete(user.Login).Result;
            
            Assert.IsTrue(result);    
        }

        [Test]
        public void SelectTest()
        {
            context.Collection.InsertOne(user);

            var result = userRepository.Get(user.Login).Result;
            
            Assert.That(result, Is.EqualTo(user).Using(new UserComparer()));
        }

        [Test]
        public void ContainsTest()
        {
            context.Collection.InsertOne(user);

            var result = userRepository.Contains(user.Login).Result;
            
            Assert.IsTrue(result);
        }
    }
}
