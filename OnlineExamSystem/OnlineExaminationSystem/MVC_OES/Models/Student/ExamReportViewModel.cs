using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Student
{
    public class ExamReportViewModel
    {
        public int courseId { get; set; }
        public int totalQuestions { get; set; }
        public int totalMarks { get; set; }
        public int correctAnswers { get; set; }
        public int score { get; set; }
        public bool isPassed { get; set; }
        public int nextLevel { get; set; }
        public bool isLastLevel { get; set; }
    }
}