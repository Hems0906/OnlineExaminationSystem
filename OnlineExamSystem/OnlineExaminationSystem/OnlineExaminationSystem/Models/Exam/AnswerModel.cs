using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExaminationSystem.Models.Exam
{
    public class AnswerModel
    {
        public int questionId { get; set; }
        public string selectedOption { get; set; }
    }
}