using MongoDB.Bson;
using NUnit.Framework;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Models.Interfaces;

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
            
            public override bool Equals(object obj)
            {
                return base.Equals(obj) || obj is Value other &&  Id == other.Id;
            }
        }
        
        private Repository<Value> repository;
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
            
            repository = new Repository<Value>(settings);
        
            context = new Context<Value>(settings);
        }

        [TearDown]
        public void Cleare()
        {
            context.Collection.Database.DropCollection(settings.CollectionName);
        }

        [Test]
        public void InsertTest()
        {
            var value = new Value();
            Assert.DoesNotThrow(() => repository.Insert(value).Wait());
        }

        [Test]
        public void DeleteTest()
        {
            var value = new Value();
            
            context.Collection.InsertOne(value);

            var result = repository.Delete(value.Id).Result;
            
            Assert.IsTrue(result);
        }

        [Test]
        public void SelectTest()
        {
            var value = new Value();
            
            context.Collection.InsertOne(value);

            var result = repository.Get(value.Id).Result;
            
            Assert.AreEqual(value, result);
        }

        [Test]
        public void ContainsTest()
        {
            var value = new Value();
            
            context.Collection.InsertOne(value);

            var result = repository.Contains(value.Id).Result;
            
            Assert.IsTrue(result);
        }
    }
}
