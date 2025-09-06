using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Controllers.Admin
{
    [RoutePrefix("api/admin")]
    public class QuestionController : ApiController
    {

        OnlineExamSystemEntities1 db = new OnlineExamSystemEntities1();

        [HttpGet]
        [Route("getQuestions/{courseId}/{levelNumber}")]
        public IHttpActionResult GetQuestions(int courseId, int levelNumber)
        {
            var questions = db.Questions
                .Where(q => q.CourseId == courseId && q.LevelNumber == levelNumber /*&& q.Status == true*/)
                .Select(q => new Models.Questions.QuestionModel
                {
                    QuestionId = q.QuestionId,
                    CourseId = q.CourseId ?? 0,
                    LevelNumber = q.LevelNumber ?? 0,
                    QuestionText = q.QuestionText,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    Answer = q.Answer,
                    Marks = q.Marks,
                    Status = q.Status
                })
                .ToList();

            return Ok(questions);
        }

        [HttpPost]
        [Route("addQuestion/{cid}/{lid}")]
        public IHttpActionResult AddQuestion(int cid, int lid, [FromBody] Models.Questions.QuestionModel model)
        {
            if (model == null)
                return BadRequest("Invalid question data.");

            if (string.IsNullOrWhiteSpace(model.QuestionText))
                return BadRequest("Question text is required.");

            if (!"ABCD".Contains(model.Answer))
                return BadRequest("Answer must be A, B, C, or D.");

            var courseExists = db.courses.Any(c => c.course_Id == cid && c.status == true);
            if (!courseExists)
                return BadRequest("Invalid CourseId. Course does not exist or is inactive.");

            var levelExists = db.Levels.Any(l => l.course_id == cid && l.level_number == lid);
            if (!levelExists)
                return BadRequest("Invalid LevelNumber. Level does not exist for the selected course.");

            var duplicate = db.Questions.Any(q =>
                q.CourseId == cid &&
                q.LevelNumber == lid &&
                q.QuestionText.Trim().ToLower() == model.QuestionText.Trim().ToLower() &&
                q.Status == true
            );

            if (duplicate)
                return BadRequest("This question already exists for the selected course and level.");

            var question = new Question
            {
                CourseId = cid,
                LevelNumber = lid,
                QuestionText = model.QuestionText,
                OptionA = model.OptionA,
                OptionB = model.OptionB,
                OptionC = model.OptionC,
                OptionD = model.OptionD,
                Answer = model.Answer,
                Marks = model.Marks,
                Status = true,
            };

            db.Questions.Add(question);
            db.SaveChanges();

            return Ok(new
            {
                message = "Question added successfully!",
                questionId = question.QuestionId
            });
        }

        [HttpPut]
        [Route("editQuestion/{id}")]
        public IHttpActionResult UpdateQuestion(int id, [FromBody] Models.Questions.QuestionModel model)
        {
            var question = db.Questions.FirstOrDefault(q => q.QuestionId == id);
            if (question == null)
                return NotFound();

            question.QuestionText = model.QuestionText;
            question.OptionA = model.OptionA;
            question.OptionB = model.OptionB;
            question.OptionC = model.OptionC;
            question.OptionD = model.OptionD;
            question.Answer = model.Answer;
            question.Marks = model.Marks;

            db.SaveChanges();

            return Ok(new { message = "Question updated successfully!" });
        }

        [HttpPut]
        [Route("toggleQuestion/{id}")]
        public IHttpActionResult ToggleQuestionStatus(int id)
        {
            var question = db.Questions.FirstOrDefault(q => q.QuestionId == id);

            if (question == null)
                return NotFound();

            question.Status = !question.Status;

            db.SaveChanges();

            return Ok(new
            {
                message = "Question Status Updated Successfully",
                newStatus = question.Status
            });
        }

        [HttpPost]
        [Route("addQuestionsBulk/{cid}/{lid}")]
        public IHttpActionResult AddQuestionsBulk(int cid, int lid, [FromBody] List<Models.Questions.QuestionModel> questions)
        {
            if (questions == null || !questions.Any())
                return BadRequest("No questions provided.");

            var addedQuestions = new List<Question>();

            foreach (var model in questions)
            {
                if (db.Questions.Any(q =>
                    q.CourseId == cid &&
                    q.LevelNumber == lid &&
                    q.QuestionText.Trim().ToLower() == model.QuestionText.Trim().ToLower() &&
                    q.Status == true))
                {
                    continue; 
                }

                var question = new Question
                {
                    CourseId = cid,
                    LevelNumber = lid,
                    QuestionText = model.QuestionText,
                    OptionA = model.OptionA,
                    OptionB = model.OptionB,
                    OptionC = model.OptionC,
                    OptionD = model.OptionD,
                    Answer = model.Answer,
                    Marks = model.Marks,
                    Status = true
                };

                db.Questions.Add(question);
                addedQuestions.Add(question);
            }

            db.SaveChanges();

            return Ok(new { message = $"{addedQuestions.Count} questions added successfully." });
        }

    }
}
