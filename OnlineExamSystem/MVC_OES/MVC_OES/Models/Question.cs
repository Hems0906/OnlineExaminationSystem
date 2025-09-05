using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string QuestionType { get; set; } // "MCQ" or "TrueFalse"

        public string OptionA { get; set; }

        public string OptionB { get; set; }

        public string OptionC { get; set; }

        public string OptionD { get; set; }

        [Required]
        public string CorrectAnswer { get; set; } // A, B, C, D or "True"/"False"
    }
}
