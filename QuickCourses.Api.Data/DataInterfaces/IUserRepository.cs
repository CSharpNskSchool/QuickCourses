using System.Threading.Tasks;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IUserRepository
    {
        Task<User> Get(Account account);
    }
}
