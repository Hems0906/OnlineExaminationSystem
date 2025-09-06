using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_OES.Models.Levels;

namespace MVC_OES.Models.Student
{
    public class CourseModel
    {
        //public int course_id { get; set; }
        //public string course_name { get; set; }
        //public int next_level { get; set; }
        //public List<LevelViewModel> levels { get; set; }
        //public string Status { get; set; } 
        //public bool CanStartExam { get; set; }

        public int course_id { get; set; }
        public string course_name { get; set; }
        public int next_level { get; set; }
        public List<LevelViewModel> levels { get; set; } 
        public string Status { get; set; }
        public bool CanStartExam { get; set; }
    }
}