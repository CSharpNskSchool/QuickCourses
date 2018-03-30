using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Data.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private static ConcurrentDictionary<ObjectId, Course> courses;

        static CourseRepository()
        {
            courses = new ConcurrentDictionary<ObjectId, Course>
            {
                [ObjectId.GenerateNewId()] = new Course
                {
                    Id = ObjectId.GenerateNewId(),
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
                                        Article = "You must love this API"
                                    },
                                    Questions = new List<Question>
                                    {
                                        new Question
                                        {
                                            Text = "Do you love this API?",
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

        public Task<Course> Get(ObjectId id)
        {
            return Task.Run(() =>
            {
                courses.TryGetValue(id, out var result);
                return result;
            });
        }
    }
}
