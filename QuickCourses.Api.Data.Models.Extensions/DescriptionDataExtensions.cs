using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class DescriptionDataExtensions
    {
        public static Description ToApiModel(this DescriptionData descriptionData)
        {
            var result = new Description
            {
                Name = descriptionData.Name,
                Overview = descriptionData.Overview
            };

            return result;
        }
    }
}
