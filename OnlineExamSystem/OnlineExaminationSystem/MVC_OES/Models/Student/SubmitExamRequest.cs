using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Student
{
    public class SubmitExamRequest
    {
        public int attemptId { get; set; }     
        public int userId { get; set; }      
        public int courseId { get; set; }     
        public int levelNumber { get; set; }  
        public int timeTaken { get; set; }   
        public List<Answer> answers { get; set; }
    }
}