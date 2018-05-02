using System.Threading.Tasks;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByLoginAsync(string login);
        Task<bool> ContainsByLoginAsync(string login);
    }
}
