using System.Threading.Tasks;
using QuickCourses.Models.Primitives;
namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IUserRepository
    {
        Task<Models.Authentication.User> Get(Models.Authentication.Account account);
        Task<User> Get(int id);
        Task<bool> Contains(int id);
        Task Insert(User user);
    }
}
