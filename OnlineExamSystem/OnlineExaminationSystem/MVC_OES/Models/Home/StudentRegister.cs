using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Home
{
    public class StudentRegister
    {
        [Required] public string StuName { get; set; }
        [Required] public string Mobile { get; set; }
        [Required] public string City { get; set; }
        [Required] public string State { get; set; }
        [Required, DataType(DataType.Date)] public DateTime DOB { get; set; }
        [Required] public string Qualification { get; set; }
        [Required] public string Completion { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
    }
}