using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private static ConcurrentDictionary<int, Course> courses;

        static CourseRepository()
        {
            courses = new ConcurrentDictionary<int, Course>
            {
                [0] = new Course
                {
                    Id = 0,
                    Description = new Description {Name = "Test Course", Overview = "Course to test Api"},
                    Lessons = new List<Lesson>
                    {
                        new Lesson
                        {
                            Id = 0,
                            Description =
                                new Description {Name = "Only lesson", Overview = "Only lesson of test course"},
                            Steps = new List<LessonStep>
                            {
                                new LessonStep
                                {
                                    Id = 0,
                                    EducationalMaterial = new EducationalMaterial
                                    {
                                        Description = new Description
                                        {
                                            Name = "Only step",
                                            Overview = "Only step of only lesson of only course"
                                        },
                                        Article = "You mast love this API"
                                    },
                                    Questions = new List<Question>
                                    {
                                        new Question
                                        {
                                            AnswerVariants = new List<AnswerVariant>
                                            {
                                                new AnswerVariant {Id = 0, Text = "Yes"}
                                            },
                                            CorrectAnswers = new List<int> {0}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        public Task<IEnumerable<Course>> GetAll()
        {
            return Task.Run(() => (IEnumerable<Course>) courses.Values);
        }

        public Task<Course> Get(int id)
        {
            return Task.Run(() =>
            {
                courses.TryGetValue(id, out var result);
                return result;
            });
        }
    }
}
