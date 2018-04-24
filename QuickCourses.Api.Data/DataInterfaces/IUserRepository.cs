using System.Threading.Tasks;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IUserRepository
    {
        Task<User> Get(string login);
        Task<bool> Contains(string login);
        Task Insert(User user);
    }
}
