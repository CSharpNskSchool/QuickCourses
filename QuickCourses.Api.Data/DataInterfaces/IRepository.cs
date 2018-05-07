using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.Data.Models.Interfaces;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IRepository<TValue>
        where TValue : IIdentifiable
    {
        Task<List<TValue>> GetAllAsync();
        Task<TValue> GetAsync(string id);
        Task<bool> ContainsAsync(string id);
        Task ReplaceAsync(string id, TValue newValue);
        Task<string> InsertAsync(TValue value);
        Task<bool> DeleteAsync(string id);
    }
}