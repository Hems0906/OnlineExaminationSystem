using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Student
{
    public class SuggestionViewModel
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        [Required]
        public string SuggestionText { get; set; }
    }
}