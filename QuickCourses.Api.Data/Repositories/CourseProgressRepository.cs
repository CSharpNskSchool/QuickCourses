using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.Repositories
{
    public class CourseProgressRepository : ICourseProgressRepository
    {
        private static ConcurrentDictionary<int, ConcurrentDictionary<int, CourseProgress>> courseProgresses;

        static CourseProgressRepository()
        {
            courseProgresses = new ConcurrentDictionary<int, ConcurrentDictionary<int, CourseProgress>>();
        }

        public Task<CourseProgress> Get(int userId, int courseId)
        {
            return Task.Run(() =>
            {
                if (!courseProgresses.TryGetValue(userId, out var userCourses))
                {
                    return null;
                }

                userCourses.TryGetValue(courseId, out var result);
                return result;
            });
        }

        public Task<bool> Contains(int userId, int courseId)
        {
            return Task.Run(() => {
                if (!courseProgresses.TryGetValue(userId, out var courses))
                {
                    return false;
                }

                return courses.ContainsKey(courseId);
            });
        }

        public Task<IEnumerable<CourseProgress>> GetAll(int userId)
        {
            return Task.Run(() => {
                if (!courseProgresses.TryGetValue(userId, out var courses))
                {
                    return new List<CourseProgress>();
                }

                return (IEnumerable<CourseProgress>)courses.Values;
            });
        }

        public Task Update(CourseProgress courseProgress)
        {
            return Task.Run(() =>
            {
                if(!courseProgresses.TryGetValue(courseProgress.UserId, out var courses))
                {
                    return;
                }

                courses.AddOrUpdate(courseProgress.CourceId, courseProgress, (courseId, progress) => courseProgress);
            });
        }

        public Task Insert(CourseProgress courseProgress)
        {
            return Task.Run(() => {
                if (!courseProgresses.TryGetValue(courseProgress.UserId, out var courses))
                {
                    courses = new ConcurrentDictionary<int, CourseProgress>();
                    courseProgresses.TryAdd(courseProgress.UserId, courses);
                }

                courses.TryAdd(courseProgress.CourceId, courseProgress);
            });
        }
    }
}
