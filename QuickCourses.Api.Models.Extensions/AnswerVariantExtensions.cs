using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Models.Extensions
{
    public static class AnswerVariantExtensions
    {
        public static AnswerVariantData ToDataModel(this AnswerVariant answerVariant)
        {
            var result = new AnswerVariantData
            {
                Id = answerVariant.Id,
                Text = answerVariant.Text
            };

            return result;
        }
    }
}
