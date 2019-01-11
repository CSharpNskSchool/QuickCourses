using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Interfaces;

namespace QuickCourses.Api.Data.Repositories
{
    public class RepositoryBase<TValue> : IRepository<TValue>
        where TValue : IIdentifiable
    {
        protected readonly DbContext<TValue> DbContext;

        public RepositoryBase(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            DbContext = new DbContext<TValue>(settings);
        }

        public virtual async Task<List<TValue>> GetAllAsync()
        {
            var result = await DbContext.Collection.Find(_ => true).ToListAsync();
            return result;
        }

        public virtual async Task<TValue> GetAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await DbContext.Collection.Find(value => value.Id == id).FirstOrDefaultAsync();
            return result;
        }

        public virtual async Task<bool> ContainsAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var value = await GetAsync(id);
            return value != null;
        }

        public virtual async Task ReplaceAsync(string id, TValue newValue)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (newValue == null)
            {
                throw new ArgumentNullException(nameof(newValue));
            }

            await DbContext.Collection.ReplaceOneAsync(value => value.Id == id, newValue);
        }

        public virtual async Task InsertAsync(TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            await DbContext.Collection.InsertOneAsync(value);
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await DbContext.Collection.DeleteOneAsync(value => value.Id == id);
            return result.DeletedCount == 1;
        }

        public string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}