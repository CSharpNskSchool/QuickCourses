using QuickCourses.Model.Primitives;
using System.Threading.Tasks;

namespace QuickCourses.Api.DataInterfaces
{
    public interface IUserRepository
    {
        Task<User> Get(int id);
        Task<bool> Contains(int id);
        Task Insert(User user);
    }
}
