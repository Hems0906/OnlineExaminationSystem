using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Profile
{
    public class StudentProfileDto
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Full Name")]
        public string stu_name { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter a valid 10-digit mobile number")]
        [Display(Name = "Mobile Number")]
        public string mobile { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string city { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Qualification is required")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Completion Year is required")]
        [Range(2000, 2050, ErrorMessage = "Enter a valid year")]
        public int Completion { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; }
    }
}