using System.Collections.Concurrent;
using System.Threading.Tasks;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Data.Repositories
{
    public class UserRepositoryDict : IUserRepository
    {
        private static readonly ConcurrentDictionary<string, User> Users;

        static UserRepositoryDict()
        {
            var user = new User
            {
                Login = "mihail",
                Password = "sexbandit",
                Id = "Krisha",
                Name = "Misha",
                Role = "User"
            };

            var userClient = new User
            {
                Login = "bot",
                Password = "12345",
                Id = "bot",
                Role = "Client",
                Name = "bot"
            };

            Users = new ConcurrentDictionary<string, User>
            {
                [user.Login] = user,
                [userClient.Login] = userClient
            };
        }

        public Task<User> Get(string login)
        {
            return Task.Run(() =>
            {
                Users.TryGetValue(login, out var result);
                return result;
            });
        }

        public Task<bool> Contains(string login)
        {
            return Task.Run(() => Users.ContainsKey(login));
        }

        public Task Insert(User user)
        {
            user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            return Task.Run(() => Users.TryAdd(user.Login, user));
        }

        public Task Delete(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
