using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Student
{
    public class LevelViewModel
    {
        public int level_number { get; set; }
        public string level_name { get; set; }
        public int tot_ques { get; set; }
        public int duration { get; set; }
        public int passing_marks { get; set; }
    }
}