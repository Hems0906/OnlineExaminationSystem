using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace mvcfinal.Models
{
    public class InstitutionViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Institution name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}