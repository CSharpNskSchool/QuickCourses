using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Interaction
{
    public class Answer
    {
        [Required]
        public int? QuestionId { get; set; }
        [Required]
        public List<int> SelectedAnswers { get; set; }
    }
}
