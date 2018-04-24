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
        private static readonly ConcurrentDictionary<string, Course> Courses;

        static CourseRepository()
        {
            var id = ObjectId.GenerateNewId().ToString();

            Courses = new ConcurrentDictionary<string, Course>
            {
                [id] = new Course {
                    Id = id,
                    Description = new Description {
                        Name = "Вводный курс!",
                        Overview = "Курс, который познакомит вас с QucickCourses."
                    },
                    Lessons = new List<Lesson>
                    {
                        new Lesson
                        {
                            CourseId = id,
                            Id = 0,
                            Description = new Description {Name = "QuickCourses", Overview = "Устройство курсов"},
                            Steps = new List<LessonStep>
                            {
                                new LessonStep
                                {
                                    CourseId = id,
                                    LessonId = 0,
                                    Id = 0,
                                    EducationalMaterial = new EducationalMaterial
                                    {
                                        Description = new Description
                                        {
                                            Name = "Курсы",
                                            Overview = "Что вообще происходит?"
                                        },
                                        Article =
                                            "QuickCourses предоставляет возможность проходить короткие курсы. Стоя в очереди или в дороге теперь можно получать знания, а не терять время"
                                    },
                                    Questions = new List<Question>
                                    {
                                        new Question
                                        {
                                            CourseId = id,
                                            LessonId = 0,
                                            LessondStepId = 0,
                                            Text = "Как вам идея?",
                                            AnswerVariants = new List<AnswerVariant>
                                            {
                                                new AnswerVariant {Id = 0, Text = "Мечта всей моей жизни"}
                                            },
                                            CorrectAnswers = new List<int> {0},
                                            TotalAttemptsCount = 2
                                        }
                                    }
                                },
                                new LessonStep
                                {
                                    CourseId = id,
                                    LessonId = 0,
                                    Id = 1,
                                    EducationalMaterial = new EducationalMaterial
                                    {
                                        Description = new Description
                                        {
                                            Name = "Уроки",
                                            Overview = "Уроки? Это как в школе, да?"
                                        },
                                        Article =
                                            "Каждый курс состоит из уроков, которые объединяют материал по определенной теме?"
                                    },
                                    Questions = new List<Question>
                                    {
                                        new Question
                                        {
                                            CourseId = id,
                                            LessonId = 0,
                                            LessondStepId = 1,
                                            Id = 0,
                                            Text = "Понятно?",
                                            AnswerVariants = new List<AnswerVariant>
                                            {
                                                new AnswerVariant {Id = 0, Text = "Да, сэр!"}
                                            },
                                            CorrectAnswers = new List<int> {0},
                                            TotalAttemptsCount = 2
                                        },
                                        new Question
                                        {
                                            CourseId = id,
                                            LessonId = 0,
                                            LessondStepId = 1,
                                            Id = 1,
                                            Text = "Уверен?!",
                                            AnswerVariants = new List<AnswerVariant>
                                            {
                                                new AnswerVariant {Id = 0, Text = "Так точно, сэр!"},
                                                new AnswerVariant {Id = 1, Text = "Не совсем!"}
                                            },
                                            CorrectAnswers = new List<int> {0},
                                            TotalAttemptsCount = 2
                                        }
                                    }
                                },
                                new LessonStep
                                {
                                    CourseId = id,
                                    LessonId = 0,
                                    Id = 2,
                                    EducationalMaterial = new EducationalMaterial
                                    {
                                        Description = new Description
                                        {
                                            Name = "Шаги",
                                            Overview = "Топ-топ-топ"
                                        },
                                        Article =
                                            "Уроки в свою очередь делятся на шаги. Каждый шаг это небольшая статья, которая завершается одним или несколькими вопросами для закрепления материала!"
                                    },
                                    Questions = new List<Question>
                                    {
                                        new Question
                                        {
                                            CourseId = id,
                                            LessonId = 0,
                                            LessondStepId = 2,
                                            Text = "Всё понятно?",
                                            AnswerVariants = new List<AnswerVariant>
                                            {
                                                new AnswerVariant {Id = 0, Text = "Абсолютно!"}
                                            },
                                            CorrectAnswers = new List<int> {0},
                                            TotalAttemptsCount = 2
                                        }
                                    }
                                }
                            }
                        },
                        new Lesson
                        {
                            CourseId = id,
                            Id = 1,
                            Description =
                                new Description {Name = "Свои курсы", Overview = "Добавление курсов"},
                            Steps = new List<LessonStep>
                            {
                                new LessonStep
                                {
                                    CourseId = id,
                                    LessonId = 1,
                                    Id = 0,
                                    EducationalMaterial = new EducationalMaterial
                                    {
                                        Description = new Description
                                        {
                                            Name = "Добавление курсов",
                                            Overview = "Как добавить свой курс?"
                                        },
                                        Article = "Никак ;)"
                                    },
                                    Questions = new List<Question>
                                    {
                                        new Question
                                        {
                                            CourseId = id,
                                            LessonId = 1,
                                            LessondStepId = 0,
                                            Text = "Что вы об этом думаете?",
                                            AnswerVariants = new List<AnswerVariant>
                                            {
                                                new AnswerVariant {Id = 0, Text = "Идеально!"}
                                            },
                                            CorrectAnswers = new List<int> {0},
                                            TotalAttemptsCount = 2
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
            return Task.Run(() =>
            {
                var result = Courses.Values;
                return (IEnumerable<Course>)result;
            });
        }

        public Task<Course> Get(string id)
        {
            return Task.Run(() =>
            {
                Courses.TryGetValue(id, out var result);
                return result;
            });
        }
    }
}
