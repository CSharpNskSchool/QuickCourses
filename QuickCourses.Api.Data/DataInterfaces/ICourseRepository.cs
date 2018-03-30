using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAll();
        Task<Course> Get(ObjectId id);
    }
}
