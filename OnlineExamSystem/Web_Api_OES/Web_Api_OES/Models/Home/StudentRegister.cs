using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Api_OES.Models.Home
{
    public class StudentRegister
    {
        public string StuName { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime DOB { get; set; }
        public string Qualification { get; set; }
        public string Completion { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}