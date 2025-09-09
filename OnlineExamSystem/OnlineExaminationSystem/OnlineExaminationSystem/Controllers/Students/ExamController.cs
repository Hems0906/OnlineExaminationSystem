using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Controllers.Students
{
    [RoutePrefix("api/exam")]
    public class ExamController : ApiController
    {
        OnlineExamSystemEntities1 db = new OnlineExamSystemEntities1();

        [HttpGet]
        [Route("instructions/{courseid}/{levelnumber}")]
        public IHttpActionResult GetInstructions(int courseId, int levelNumber)
        {
            try
            {
                var course = db.courses.FirstOrDefault(c => c.course_Id == courseId);
                if (course == null)
                    return NotFound();

                var level = db.Levels.FirstOrDefault(l => l.course_id == courseId && l.level_number == levelNumber);
                if (level == null)
                    return NotFound();

                var result = new
                {
                    course_id = course.course_Id,
                    course_name = course.course_name,
                    level_number = level.level_number,
                    level_name = level.level_name,
                    tot_ques = level.tot_ques,
                    duration = level.duration,
                    passing_marks = level.passing_marks
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("start/{userId}/{courseId}/{levelNumber}")]
        public IHttpActionResult StartExam(int userId, int courseId, int levelNumber)
        {
            try
            {
                var level = db.Levels.FirstOrDefault(l => l.course_id == courseId && l.level_number == levelNumber);
                if (level == null)
                    return BadRequest("Level not found");

                var questions = db.Questions
                                  .Where(q => q.CourseId == courseId && q.LevelNumber == levelNumber && q.Status == true)
                                  .OrderBy(q => Guid.NewGuid()) 
                                  .Take(level.tot_ques)
                                  .Select(q => new
                                  {
                                      q.QuestionId,
                                      q.QuestionText,
                                      q.OptionA,
                                      q.OptionB,
                                      q.OptionC,
                                      q.OptionD,
                                      q.Marks  
                                    }).ToList();

                if (!questions.Any())
                    return BadRequest("No questions available for this level.");

                var examAttempt = new ExamAttempt
                {
                    user_id = userId,
                    course_id = courseId,
                    level_number = levelNumber,
                    total_questions = level.tot_ques,
                    correct_answers = 0,
                    score = 0,
                    total_time = level.duration,
                    time_taken = 0,
                    is_passed = false
                };

                db.ExamAttempts.Add(examAttempt);
                db.SaveChanges();

                var result = new
                {
                    attempt_id = examAttempt.attempt_id,
                    user_id = userId,                 
                    course_id = courseId,
                    level_number = levelNumber,
                    duration = level.duration,
                    total_questions = level.tot_ques,  
                    questions = questions
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("submit")]
        public IHttpActionResult SubmitExam([FromBody] Models.Exam.SubmitExamRequest request)
        {
            try
            {
                if (request == null || request.answers == null)
                    return BadRequest("Invalid request");

                var level = db.Levels.FirstOrDefault(l => l.course_id == request.courseId && l.level_number == request.levelNumber);
                if (level == null)
                    return BadRequest("Level not found");

                var answeredQIds = request.answers.Select(a => a.questionId).ToList();

                var questions = db.Questions
                    .Where(q => answeredQIds.Contains(q.QuestionId))
                    .ToList();

                int totalMarks = questions.Sum(q => q.Marks);  
                int score = 0;  
                int correctAnswers = 0;

                var attempt = db.ExamAttempts.FirstOrDefault(a => a.attempt_id == request.attemptId);
                if (attempt == null)
                    return BadRequest("Exam attempt not found");

                foreach (var ans in request.answers)
                {
                    var q = questions.FirstOrDefault(x => x.QuestionId == ans.questionId);
                    if (q == null) continue;

                    bool isCorrect = !string.IsNullOrEmpty(ans.selectedOption) &&
                                     string.Equals(ans.selectedOption, q.Answer, StringComparison.OrdinalIgnoreCase);

                    if (isCorrect)
                    {
                        correctAnswers++;
                        score += q.Marks;  
                    }

                    var userAns = new UserAnswer
                    {
                        attempt_id = attempt.attempt_id,
                        question_id = q.QuestionId,
                        selected_option = string.IsNullOrEmpty(ans.selectedOption) ? null : ans.selectedOption,
                        is_correct = isCorrect
                    };
                    db.UserAnswers.Add(userAns);
                }
                db.SaveChanges();

                bool isPassed = score >= level.passing_marks;

                attempt.correct_answers = correctAnswers;
                attempt.score = score;
                attempt.total_questions = request.answers.Count;
                attempt.total_time = level.duration;
                attempt.time_taken = request.timeTaken;
                attempt.is_passed = isPassed;
                db.SaveChanges();

                var progress = db.StudentProgresses.FirstOrDefault(p => p.user_id == request.userId && p.course_id == request.courseId);
                if (progress == null)
                {
                    progress = new StudentProgress
                    {
                        user_id = request.userId,
                        course_id = request.courseId,
                        highest_level_passed = isPassed ? request.levelNumber : 0,
                        is_completed = false
                    };
                    db.StudentProgresses.Add(progress);
                }
                else if (isPassed)
                {
                    progress.highest_level_passed = request.levelNumber;
                }

                int maxLevel = db.Levels.Where(l => l.course_id == request.courseId).Max(l => l.level_number);
                progress.is_completed = isPassed && request.levelNumber == maxLevel;
                db.SaveChanges();

                var report = new ExamReport
                {
                    attempt_id = attempt.attempt_id,
                    user_id = request.userId,
                    course_id = request.courseId,
                    level_number = request.levelNumber,
                    total_marks = totalMarks,
                    passing_marks = level.passing_marks,
                    score = score,
                    is_passed = isPassed,
                    total_time = level.duration,
                    time_taken = request.timeTaken
                };
                db.ExamReports.Add(report);
                db.SaveChanges();

                var response = new
                {
                    totalQuestions = request.answers.Count,
                    totalMarks = totalMarks,
                    correctAnswers = correctAnswers,
                    score = score,
                    isPassed = isPassed,
                    nextLevel = request.levelNumber + 1,
                    isLastLevel = request.levelNumber == maxLevel
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}

