using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Student
{
    public class Answer
    {
        public int questionId { get; set; }    
        public string selectedOption { get; set; }
    }
}