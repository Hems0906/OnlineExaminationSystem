using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Questions
{
    public class ExamAttemptViewModel
    {
        public int AttemptId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
        public string CourseName { get; set; }
        public int LevelNumber { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int Score { get; set; }
        public int TotalTime { get; set; }
        public int TimeTaken { get; set; }
        public bool IsPassed { get; set; }
    }
}