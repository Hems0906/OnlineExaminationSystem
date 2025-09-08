using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Questions
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }

        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public string LevelName { get; set; }

        public int LevelNumber { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string OptionA { get; set; }

        [Required]
        public string OptionB { get; set; }

        [Required]
        public string OptionC { get; set; }

        [Required]
        public string OptionD { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public int Marks { get; set; }

        public bool Status { get; set; }
    }
}