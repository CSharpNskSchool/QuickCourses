using System.Linq;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Models.Progress;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class StepProgressExtensions
    {
        public static StepProgress ToApiModel(this StepProgressData stepProgressData)
        {
            var result = new StepProgress
            {
                ProgressId = stepProgressData.ProgressId,
                CourseId = stepProgressData.CourseId,
                LessonId = stepProgressData.LessonId,
                Passed = stepProgressData.Passed,
                QuestionStates = stepProgressData.QuestionStates
                    .Select(state => state.ToApiModel())
                    .ToList(),
                Id = stepProgressData.Id
            };

            return result;
        }
    }
}
