using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Questions
{
    public class QuestionModel
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string LevelName { get; set; }

        [Required]
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

        [Required]
        public bool Status { get; set; }
    }
}