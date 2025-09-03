using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace mvcfinal.Models
{
    public class QuestionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Option A is required")]
        public string OptionA { get; set; }

        [Required(ErrorMessage = "Option B is required")]
        public string OptionB { get; set; }

        [Required(ErrorMessage = "Option C is required")]
        public string OptionC { get; set; }

        [Required(ErrorMessage = "Option D is required")]
        public string OptionD { get; set; }

        [Required(ErrorMessage = "Select correct answer")]
        public string CorrectAnswer { get; set; }

        [Required(ErrorMessage = "Select level")]
        [Range(1, 3)]
        public int Level { get; set; } // 1, 2, 3
    }
    public class SubjectViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Subject name is required")]
        public string Name { get; set; }

        public int Level1Timer { get; set; } // in minutes
        public int Level2Timer { get; set; }
        public int Level3Timer { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }
}
