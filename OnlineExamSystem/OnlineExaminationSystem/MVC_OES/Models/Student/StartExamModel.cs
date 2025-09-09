using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_OES.Models.Student;

namespace MVC_OES.Models.Student
{
    public class StartExamModel
    {
        public int user_id { get; set; }
        public int attempt_id { get; set; }
        public int course_id { get; set; }
        public int level_number { get; set; }
        public int duration { get; set; } 
        public List<QuestionModel> questions { get; set; }
    }
}