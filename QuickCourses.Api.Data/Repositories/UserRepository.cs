using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Context<User> context;

        public UserRepository(Settings settings)
        {
            context = new Context<User>(settings, collectionName: "Users");
        }

        public async Task<User> Get(string login)
        {
            var result = await context.Collection.Find(x => x.Login == login).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> Contains(string login)
        {
            var user = await Get(login);
            return user != null;
        }

        public async Task Insert(User user)
        {
            user.Id = ObjectId.GenerateNewId().ToString();
            await context.Collection.InsertOneAsync(user);
        }

        public async Task Delete(string login)
        {
            await context.Collection.DeleteOneAsync(x => x.Login == login);
        }
    }
}
