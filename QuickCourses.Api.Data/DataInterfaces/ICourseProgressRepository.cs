using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.DataInterfaces
{
    public interface ICourseProgressRepository
    {
        Task<CourseProgress> Get(int userId, ObjectId courseId);
        Task<bool> Contains(int userId, ObjectId courseId);
        Task<IEnumerable<CourseProgress>> GetAll(int userId);
        Task Update(CourseProgress courseProgress);
        Task Insert(CourseProgress courseProgress);
    }
}
