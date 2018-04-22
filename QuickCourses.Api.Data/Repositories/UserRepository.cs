using System.Threading.Tasks;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User> Get(Account account)
        {
            return Task.Run(() => 
            {
                if (account.Login == "mihail" && account.Password == "sexbandit")
                {
                    return new User
                    {
                        Account = account,
                        Id = "Krisha",
                        Name = "Misha",
                        Role = "User"
                    };
                }

                return null;
            });
        }
    }
}
