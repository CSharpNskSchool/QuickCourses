using System;
using System.Collections.Generic;
using System.Text;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Models.Extensions
{
    public static  class EducationalMaterialExtensions
    {
        public static EducationalMaterialData ToDataModel(this EducationalMaterial educationalMaterial)
        {
            var result = new EducationalMaterialData
            {
                Article = educationalMaterial.Article,
                DescriptionData = educationalMaterial.Description.ToDataModel()
            };

            return result;
        }
    }
}
