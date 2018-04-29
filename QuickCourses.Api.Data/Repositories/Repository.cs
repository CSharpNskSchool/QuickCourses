using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Models.Interfaces;

namespace QuickCourses.Api.Data.Repositories
{
    public class Repository<TValue> : IRepository<TValue>
        where TValue : IValueWithId
    {
        protected readonly Context<TValue> context;

        public Repository(Settings settings)
        {
            context = new Context<TValue>(settings);
        }
        
        public virtual async Task<IEnumerable<TValue>> GetAll()
        {
            var result = await context.Collection.Find(_ => true).ToListAsync();
            return result;
        }

        public virtual async Task<TValue> Get(string id)
        {
            var result = await context.Collection.Find(value => value.Id == id).FirstOrDefaultAsync();
            return result;
        }

        public virtual async Task<bool> Contains(string id)
        {
            var value = await Get(id);
            return value != null;
        }

        public virtual async Task Replace(string id, TValue newValue)
        {
            await context.Collection.ReplaceOneAsync(value => value.Id == id, newValue);
        }

        public virtual async Task Insert(TValue value)
        {
            await context.Collection.InsertOneAsync(value);
        }

        public virtual async Task<bool> Delete(string id)
        {
            try
            {
                var result = await context.Collection.DeleteOneAsync(x => x.Id == id);
                return result.DeletedCount == 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}