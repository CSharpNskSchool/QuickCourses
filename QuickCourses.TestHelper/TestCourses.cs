using QuickCourses.Models.Primitives;
using System.Collections.Generic;

namespace QuickCourses.TestHelper
{
    public class TestCourses
    {
        public static Course CreateBasicSample()
        {
            return new Course
            {
                Id = "88a0eed0-6980-4264-8ae4-08c22e701fd7", //some GUID
                Description = new Description { Name = "Test Course", Overview = "Course to test Api" },
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
                                        CorrectAnswers = new List<int> {0},
                                        TotalAttemptsCount = 2
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
