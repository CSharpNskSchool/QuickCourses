using System.Threading.Tasks;
using QuickCourses.Api.Data.Models.Authentication;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface IUserRepository : IRepository<UserData>
    {
        Task<UserData> GetByLoginAsync(string login);
        Task<bool> ContainsByLoginAsync(string login);
    }
}
