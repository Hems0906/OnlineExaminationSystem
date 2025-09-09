using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineExaminationSystem.Models.Exam;

namespace OnlineExaminationSystem.Models.Exam
{
    public class SubmitExamRequest
    {
        public int attemptId { get; set; }
        public int userId { get; set; }
        public int courseId { get; set; }
        public int levelNumber { get; set; }
        public List<AnswerModel> answers { get; set; }
        public int timeTaken { get; set; }
    }
}