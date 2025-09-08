using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Models.Home
{
    public class LoginResponse
    {
        public int userId { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public int stuId { get; set; }
        public string userName { get; set; }
    }
}