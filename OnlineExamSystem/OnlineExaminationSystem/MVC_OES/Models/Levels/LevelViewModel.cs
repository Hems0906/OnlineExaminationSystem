using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Levels
{
    public class LevelViewModel
    {
        public int LevelId { get; set; }
        public int CourseId { get; set; }
        public int LevelNumber { get; set; }
        public string LevelName { get; set; }
        public int PassingMarks { get; set; }
        public int TotalQuestions { get; set; }
        public int Duration { get; set; }
    }
}