using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Questions
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }
        public int CourseId { get; set; }
        public int LevelNumber { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string Answer { get; set; }
        public int Marks { get; set; }
        public bool Status { get; set; }
    }
}