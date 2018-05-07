using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Authentication;

namespace QuickCourses.Api.Data.Repositories
{
    public class UserRepository : RepositoryBase<UserData>, IUserRepository
    {
        public UserRepository(Settings settings) 
            : base(settings)
        {
        }

        public async Task<UserData> GetByLoginAsync(string login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var result = await Context.Collection.Find(user => user.Login == login).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> ContainsByLoginAsync(string login)
        {
            var user = await GetByLoginAsync(login);
            return user != null;
        }
    }
}
