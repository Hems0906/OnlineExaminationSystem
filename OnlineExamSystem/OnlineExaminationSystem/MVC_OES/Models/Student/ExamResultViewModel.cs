using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Student
{
    public class ExamResultViewModel
    {
        public string Course { get; set; }
        public string Level { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public int Score { get; set; }
        public string Result { get; set; }
        public int TotalTime { get; set; }
        public int TimeTaken { get; set; }
    }
}