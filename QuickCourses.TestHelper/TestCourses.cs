using System.Collections.Generic;
using QuickCourses.Api.Data.Models.Primitives;

namespace QuickCourses.TestHelper
{
    public class TestCourses
    {
        public static CourseData CreateBasicSample()
        {
            return new CourseData
            {
                DescriptionData = new DescriptionData { Name = "Test Course", Overview = "Course to test Api" },
                Lessons = new List<LessonData>
                {
                    new LessonData
                    {
                        Id = 0,
                        DescriptionData =
                            new DescriptionData {Name = "Only lesson", Overview = "Only lesson of test course"},
                        Steps = new List<LessonStepData>
                        {
                            new LessonStepData
                            {
                                Id = 0,
                                EducationalMaterialData = new EducationalMaterialData
                                {
                                    DescriptionData = new DescriptionData
                                    {
                                        Name = "Only step",
                                        Overview = "Only step of only lesson of only course"
                                    },
                                    Article = "You must love this API"
                                },
                                Questions = new List<QuestionData>
                                {
                                    new QuestionData
                                    {
                                        Text = "Do you love this API?",
                                        AnswerVariants = new List<AnswerVariantData>
                                        {
                                            new AnswerVariantData {Id = 0, Text = "Yes"}
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
