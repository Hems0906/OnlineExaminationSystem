using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Home
{
    public class StudentRegister
    {
        [Required(ErrorMessage = "The Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        [Display(Name = "Name")]
        public string StuName { get; set; }

        [Required(ErrorMessage = "The Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [Display(Name = "Mobile Number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "The City is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "City must be between 3 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "City can only contain letters and spaces.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "The State is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "State must be between 3 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "State can only contain letters and spaces.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "The Date of Birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "The Qualification is required.")]
        [Display(Name = "Qualification")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "The Completion Year is required.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Completion year must be 4 digits.")]
        [Display(Name = "Completion Year")]
        public string Completion { get; set; }

        [Required(ErrorMessage = "The Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and include one uppercase, one lowercase, one number, and one special character.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "The Captcha is required.")]
        [Display(Name = "Captcha")]
        public string CaptchaInput { get; set; }
    }
}