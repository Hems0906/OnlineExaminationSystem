using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Levels
{
    public class LevelViewModel
    {
        //[Required]
        public int LevelId { get; set; }

        //[Required]
        public int CourseId { get; set; }

        [Required]
        public int LevelNumber { get; set; }

        [Required]
        public string LevelName { get; set; }

        [Required]
        public int PassingMarks { get; set; }

        [Required]
        public int TotalQuestions { get; set; }

        [Required]
        public int Duration { get; set; }
    }
}