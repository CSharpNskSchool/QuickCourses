using System.Threading.Tasks;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User> Get(int id)
        {
            return Task.Run(() => default(User));
        }

        public Task<bool> Contains(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task Insert(User user)
        {
            return Task.CompletedTask;
        }
    }
}
