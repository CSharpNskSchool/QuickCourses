using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Models.Extensions
{
    public static class DescriptionExtensions
    {
        public static DescriptionData ToDataModel(this Description description)
        {
            var result = new DescriptionData
            {
                Name = description.Name,
                Overview = description.Overview
            };

            return result;
        }
    }
}
