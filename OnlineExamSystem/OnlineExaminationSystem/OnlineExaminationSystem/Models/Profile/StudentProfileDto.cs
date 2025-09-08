using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExaminationSystem.Models.Profile
{
    public class StudentProfileDto
    {
        public string stu_name { get; set; }
        public string mobile { get; set; }
        public string city { get; set; }
        public string State { get; set; }
        public DateTime DOB { get; set; }
        public string Qualification { get; set; }
        public string Completion { get; set; }
        public string email { get; set; }
    }
}