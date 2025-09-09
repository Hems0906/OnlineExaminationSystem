using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    }
}
