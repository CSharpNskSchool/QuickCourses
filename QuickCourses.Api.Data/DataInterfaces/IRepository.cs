using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Models.Interfaces;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IRepository<TValue>
        where TValue : IValueWithId
    {
        Task<IEnumerable<TValue>> GetAll();
        Task<TValue> Get(string id);
        Task<bool> Contains(string id);
        Task Replace(string id, TValue newValue);
        Task Insert(TValue value);
        Task<bool> Delete(string id);
    }
}