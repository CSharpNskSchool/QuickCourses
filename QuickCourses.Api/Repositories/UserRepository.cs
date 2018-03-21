using System.Threading.Tasks;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User> GetUser(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task Insert(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}
