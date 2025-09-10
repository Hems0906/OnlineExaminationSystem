using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExaminationSystem.Models.Exam
{
    public class SuggestionModel
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string SuggestionText { get; set; }
    }
}