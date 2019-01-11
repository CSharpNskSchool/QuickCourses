using System.Threading.Tasks;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.TelegramBot.Models;
using System;

namespace QuickCourses.TelegramBot.Data
{
    public class UserRepository : RepositoryBase<UserInfo>
    {
        public UserRepository(Settings settings) 
            : base(settings)
        {
        }

        public override async Task<string> InsertAsync(UserInfo value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Id == null)
            {
                throw new ArgumentNullException(nameof(value.Id));
            }

            await DbContext.Collection.InsertOneAsync(value);
            return value.Id;
        }
    }
}
