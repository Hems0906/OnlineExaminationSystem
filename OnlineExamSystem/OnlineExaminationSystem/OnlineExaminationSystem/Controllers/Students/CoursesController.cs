using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Web.Http;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Controllers.Students
{
    [RoutePrefix("api/courses")]
    public class CoursesController : ApiController
    {
        OnlineExamSystemEntities2 db = new OnlineExamSystemEntities2();

        [HttpGet]
        [Route("getcourses/{userId}")]
        public IHttpActionResult GetCourses(int userId)
        {
            try
            {
                var activeCourses = db.courses.Where(c => c.status == true).ToList();

                var result = new List<object>();

                foreach (var course in activeCourses)
                {
                    var levels = db.Levels
                        .Where(l => l.course_id == course.course_Id)
                        .OrderBy(l => l.level_number)
                        .ToList();

                    bool validCourse = levels.All(l =>
                        db.Questions.Count(q => q.CourseId == course.course_Id && q.LevelNumber == l.level_number && q.Status == true)
                        >= l.tot_ques
                    );

                    if (!validCourse)
                        continue;

                    var progress = db.StudentProgresses
                        .FirstOrDefault(p => p.user_id == userId && p.course_id == course.course_Id);

                    int nextLevel = (progress == null || !progress.highest_level_passed.HasValue) ? 1 : progress.highest_level_passed.Value + 1;

                    bool isCompleted = (progress != null && progress.is_completed == true);

                    var pendingLevels = levels
                        .Where(l => l.level_number >= nextLevel)
                        .Select(l => new
                        {
                            l.level_number,
                            l.level_name,
                            l.tot_ques,
                            l.duration,
                            l.passing_marks
                        })
                        .ToList();

                    result.Add(new
                    {
                        course_id = course.course_Id,
                        course_name = course.course_name,
                        next_level = nextLevel,
                        levels = pendingLevels,
                        Status = isCompleted ? "Completed" : "Not Completed",
                        CanStartExam = !isCompleted
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("getresults/{userId}")]
        public IHttpActionResult GetResults(int userId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.user_Id == userId);
                if (user == null)
                    return NotFound();

                if (user.role.ToLower() == "student")
                {
                    var stu = db.Students.FirstOrDefault(s => s.stu_id == user.reference_Id);
                    if (stu == null)
                        return NotFound();
                }
                else if (user.role.ToLower() == "admin")
                {
                    var adm = db.Admins.FirstOrDefault(a => a.admin_id == user.reference_Id);
                    if (adm == null)
                        return NotFound();
                }

                var reports = (from er in db.ExamReports
                               join c in db.courses on er.course_id equals c.course_Id
                               join l in db.Levels on er.level_number equals l.level_id
                               where er.user_id == userId
                               select new
                               {
                                   Course = c.course_name,
                                   Level = l.level_name,
                                   TotalMarks = er.total_marks,
                                   PassingMarks = er.passing_marks,
                                   Score = er.score,
                                   Result = er.is_passed ? "Pass" : "Fail",
                                   TotalTime = er.total_time,
                                   TimeTaken = er.time_taken
                               }).ToList();

                return Ok(reports);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("dashboard/{userId}")]
        public IHttpActionResult GetStudentDashboard(int userId)
        {
            try
            {
                var activeCourses = db.courses.Where(c => c.status == true).ToList();

                var validCourses = activeCourses.Where(course => db.Levels.Where(l => l.course_id == course.course_Id).All(l =>db.Questions.Count(q => q.CourseId == course.course_Id && q.LevelNumber == l.level_number && q.Status == true)>= l.tot_ques)).ToList();

                var totalCourses = validCourses.Count;

                var completedCourses = db.StudentProgresses
                    .Count(sp => sp.user_id == userId && sp.is_completed == true);

                var ongoingCourses = totalCourses - completedCourses;

                var totalAttempts = db.ExamAttempts
                    .Count(ea => ea.user_id == userId);

                var passedAttempts = db.ExamAttempts
                    .Count(ea => ea.user_id == userId && ea.is_passed);

                double passPercentage = totalAttempts > 0
                    ? Math.Round((double)passedAttempts / totalAttempts * 100, 2)
                    : 0;

                var stats = new
                {
                    TotalCourses = totalCourses,
                    CompletedCourses = completedCourses,
                    OngoingCourses = ongoingCourses,
                    TotalExamsAttempted = totalAttempts,
                    PassedExams = passedAttempts,
                    PassPercentage = passPercentage
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("completedcourse/{userId}/{courseId}")]
        public IHttpActionResult GetCompletedCourseProgress(int userId, int courseId)
        {
            try
            {
                var progress = db.StudentProgresses
                    .FirstOrDefault(sp => sp.user_id == userId && sp.course_id == courseId && sp.is_completed == true);

                if (progress == null)
                    return NotFound();

                var levels = db.Levels
                    .Where(l => l.course_id == courseId)
                    .OrderBy(l => l.level_number)
                    .Select(l => new
                    {
                        l.level_number,
                        l.level_name,
                        Attempt = db.ExamAttempts
                            .Where(a => a.user_id == userId && a.course_id == courseId && a.level_number == l.level_number && a.is_passed == true)
                            .Select(a => new
                            {
                                a.score,
                                a.total_questions,
                                a.is_passed,
                                a.time_taken,
                                a.total_time
                            })
                            .FirstOrDefault()
                    })
                    .ToList();

                var user = db.Users.FirstOrDefault(u => u.user_Id == userId);
                if (user == null)
                    return NotFound();

                string subject = $"Course Completion: {progress.cours.course_name}";
                string body = $"<h3>Congratulations {user.email}!</h3>";
                body += $"<p>You have successfully completed the course <strong>{progress.cours.course_name}</strong>.</p>";
                body += "<table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse;'>";
                body += "<tr><th>Level</th><th>Score</th><th>Total Questions</th><th>Result</th></tr>";

                foreach (var lvl in levels)
                {
                    if (lvl.Attempt != null)
                    {
                        body += $"<tr>" +
                                $"<td>{lvl.level_name} (Level {lvl.level_number})</td>" +
                                $"<td>{lvl.Attempt.score}</td>" +
                                $"<td>{lvl.Attempt.total_questions}</td>" +
                                $"<td>{(lvl.Attempt.is_passed ? "Pass" : "Fail")}</td>" +
                                $"</tr>";
                    }
                }

                body += "</table>";

                SendEmail(user.email, subject, body);

                return Ok(new { message = "Email sent successfully." });

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private void SendEmail(string to, string subject, string body)
        {
            var from = "infiniteprojecttest@gmail.com";
            var password = "punt gpsv ogqm mzjd"; 

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(from, password);
                client.EnableSsl = true;

                var mail = new MailMessage(from, to, subject, body);
                mail.IsBodyHtml = true; 
                client.Send(mail);
            }
        }

    }
}
