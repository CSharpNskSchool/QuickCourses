using QuickCourses.Model.Primitives;
using System.Threading.Tasks;

namespace QuickCourses.Api.DataInterfaces
{
    public interface IUserRepository
    {
        Task<User> GetUser(int id);
        Task Insert(User user);
    }
}
