using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Interfaces;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.Repositories
{
    public class RepositoryDict<TValue> : IRepository<TValue>
        where TValue : IValueWithId
    {
        private static readonly ConcurrentDictionary<string, TValue> Values;

        static RepositoryDict()
        {
            Values = new ConcurrentDictionary<string, TValue>();
        }

        public Task<IEnumerable<TValue>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Task<TValue> Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Contains(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task Replace(string id, TValue newValue)
        {
            throw new System.NotImplementedException();
        }

        public Task Insert(TValue value)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
