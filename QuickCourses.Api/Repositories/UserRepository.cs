using System.Collections.Concurrent;
using System.Threading.Tasks;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static ConcurrentDictionary<int, User> users;

        static UserRepository()
        {
            users = new ConcurrentDictionary<int, User>();
        }

        public Task<User> Get(int id)
        {
            return Task.Run(() =>
            {
                users.TryGetValue(id, out var result);
                return result;
            });
        }

        public Task<bool> Contains(int id)
        {
            return Task.Run(() => users.TryGetValue(id, out var result));
        }

        public Task Insert(User user)
        {
            return Task.Run(() => users.TryAdd(user.Id, user));
        }
    }
}
