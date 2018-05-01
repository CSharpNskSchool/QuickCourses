using MongoDB.Bson;
using NUnit.Framework;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Models.Interfaces;
using KellermanSoftware.CompareNetObjects;

namespace QuickCourses.Api.Data.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class RepositoryTests
    {
        private class Value : IValueWithId
        {
            public Value()
            {
                Id = ObjectId.GenerateNewId().ToString();
            }
            
            public string Id { get; set; }
        }
        
        private RepositoryBase<Value> repository;
        private Context<Value> context;
        private Settings settings;
        
        [SetUp]
        public void Init()
        {
            settings = new Settings(
                connectionString: "mongodb://localhost:27017/",
                database: "Test",
                collectionName: "Value"
            );
            
            repository = new RepositoryBase<Value>(settings);
        
            context = new Context<Value>(settings);
        }

        [TearDown]
        public void Clear()
        {
            context.Collection.Database.DropCollection(settings.CollectionName);
        }

        [Test]
        public void InsertTest()
        {
            var value = new Value();
            var result = repository.InsertAsync(value).Result;
            Assert.IsTrue(result != null);
        }

        [Test]
        public void DeleteTest()
        {
            var value = new Value();
            
            context.Collection.InsertOne(value);

            var result = repository.DeleteAsync(value.Id).Result;
            
            Assert.IsTrue(result);
        }

        [Test]
        public void SelectTest()
        {
            var value = new Value();
            
            context.Collection.InsertOne(value);

            var result = repository.GetAsync(value.Id).Result;

            var compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(value, result);

            Assert.IsTrue(compareResult.AreEqual);
        }

        [Test]
        public void ContainsTest()
        {
            var value = new Value();
            
            context.Collection.InsertOne(value);

            var result = repository.ContainsAsync(value.Id).Result;
            
            Assert.IsTrue(result);
        }
    }
}
