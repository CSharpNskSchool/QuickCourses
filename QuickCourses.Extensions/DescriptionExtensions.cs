using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class DescriptionExtensions
    {
        public static Description ToApiModel(this Primitives.DescriptionData descriptionData)
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
