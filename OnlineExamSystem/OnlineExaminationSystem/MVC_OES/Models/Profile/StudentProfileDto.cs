using System;
using System.ComponentModel.DataAnnotations;


namespace MVC_OES.Models.Profile
{
    public class StudentProfileDto
    {
        [Required]
        public string stu_name { get; set; }

        [Required, EmailAddress]
        public string email { get; set; }

        [Required, Phone]
        public string mobile { get; set; }

        public string city { get; set; }
        public string State { get; set; }

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        public string Qualification { get; set; }
        public string Completion { get; set; }
    }

}