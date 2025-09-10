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
    public class AdminController : ApiController
    {
        OnlineExamSystemEntities4 db = new OnlineExamSystemEntities4();

        [HttpGet]
        [Route("getdashboardstats")]
        public IHttpActionResult GetDashboardStats()
        {
            try
            {
                var totalStudents = db.Students.Count();
                var totalCourses = db.courses.Count();
                var totalExams = db.ExamAttempts.Count();
                var totalPassed = db.ExamAttempts.Count(e => e.is_passed);

                var passPercentage = totalExams > 0 ? (totalPassed * 100 / totalExams) : 0;

                return Ok(new
                {
                    totalStudents,
                    totalCourses,
                    totalExams,
                    passPercentage
                });

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("addcourse")]
        public IHttpActionResult AddCourse([FromBody] Models.Courses.CourseModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CourseName))
                return BadRequest("Course name is required.");


            if (db.courses.Any(c => c.course_name == model.CourseName))
                return BadRequest("Course already exists.");

            var course = new cours
            {
                course_name = model.CourseName,
                status = true
            };

            db.courses.Add(course);
            db.SaveChanges();



            return Ok(new
            {
                message = "Course added successfully",
                courseId = course.course_Id,
                courseStatus = course.status
            });
        }


        [HttpGet]
        [Route("getcourses")]
        public IHttpActionResult GetCourses()
        {
            var courses = db.courses.Select(c => new OnlineExaminationSystem.Models.Courses.CourseModel
            {
                CourseId = c.course_Id,
                CourseName = c.course_name,
                Status = c.status ?? true
            }).ToList();

            return Ok(courses);
        }

        [HttpPut]
        [Route("toggle/{id}")]
        public IHttpActionResult ToggleStatus(int id)
        {
            var course = db.courses.FirstOrDefault(c => c.course_Id == id);

            if (course == null)
            {
                return NotFound();
            }

            course.status = !course.status;
            db.SaveChanges();

            return Ok(new { message = "Course status updated successfully!", newStatus = course.status });
        }

        [HttpPut]
        [Route("edit/{id}")]
        public IHttpActionResult EditCourse(int id, [FromBody] Models.Courses.CourseModel updatedCourse)
        {
            var course = db.courses.FirstOrDefault(c => c.course_Id == id);

            if (course == null)
            {
                return NotFound();
            }

            course.course_name = updatedCourse.CourseName;
            db.SaveChanges();

            return Ok(new { message = "Course Updated Successfully" });
        }

        [HttpPost]
        [Route("addlevels/{courseId}")]
        public IHttpActionResult AddLevels(int courseId, [FromBody] List<Models.Levels.LevelsModel> levels)
        {
            if (levels == null || levels.Count == 0)
                return BadRequest("Level data is required.");

            var course = db.courses.FirstOrDefault(c => c.course_Id == courseId);
            if (course == null)
                return NotFound();

            foreach (var model in levels)
            {
                var level = new Level
                {
                    course_id = courseId,
                    level_number = model.LevelNumber,
                    level_name = model.LevelName,
                    passing_marks = model.PassingMarks,
                    tot_ques = model.TotalQuestions,
                    duration = model.Duration
                };

                db.Levels.Add(level);
            }

            db.SaveChanges();

            return Ok(new
            {
                message = "All levels added successfully",
                CourseID = courseId
            });
        }

        [HttpGet]
        [Route("getlevels/{courseId}")]
        public IHttpActionResult GetLevels(int courseId)
        {
            var levels = db.Levels
                .Where(l => l.course_id == courseId)
                .Select(l => new OnlineExaminationSystem.Models.Levels.LevelsModel
                {
                    LevelId = l.level_id,
                    CourseId = l.course_id ?? 0,
                    LevelNumber = l.level_number,
                    LevelName = l.level_name,
                    PassingMarks = l.passing_marks,
                    TotalQuestions = l.tot_ques,
                    Duration = l.duration
                })
                .ToList();

            return Ok(levels);
        }

        [HttpGet]
        [Route("getlevel/{courseId}/{levelNumber}")]
        public IHttpActionResult GetLevelByNumber(int courseId, int levelNumber)
        {
            var level = db.Levels
                .Where(l => l.course_id == courseId && l.level_number == levelNumber)
                .Select(l => new OnlineExaminationSystem.Models.Levels.LevelsModel
                {
                    LevelId = l.level_id,
                    CourseId = l.course_id ?? 0,
                    LevelNumber = l.level_number,
                    LevelName = l.level_name,
                    PassingMarks = l.passing_marks,
                    TotalQuestions = l.tot_ques,
                    Duration = l.duration
                })
                .FirstOrDefault();

            if (level == null)
                return NotFound();

            return Ok(level);
        }

        [HttpPut]
        [Route("editlevel/{courseId}/{levelNumber}")]
        public IHttpActionResult EditLevelByNumber(int courseId, int levelNumber, [FromBody] Models.Levels.LevelsModel model)
        {
            if (model == null)
                return BadRequest("Level data is required.");


            var level = db.Levels.FirstOrDefault(l => l.course_id == courseId && l.level_number == levelNumber);

            if (level == null)
                return NotFound();


            level.level_name = model.LevelName;
            level.passing_marks = model.PassingMarks;
            level.tot_ques = model.TotalQuestions;
            level.duration = model.Duration;

            db.SaveChanges();

            return Ok(new
            {
                message = "Level updated successfully",
                CourseId = courseId,
                LevelNumber = levelNumber
            });
        }

        [HttpGet]
        [Route("profile/get")]
        public IHttpActionResult GetProfile(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.user_Id == id && u.role == "admin");
            if (user == null)
                return Content(HttpStatusCode.NotFound, "User not found");

            var admin = db.Admins.FirstOrDefault(a => a.admin_id == user.reference_Id);
            if (admin == null)
                return Content(HttpStatusCode.NotFound, "Admin profile not found");

            var result = new Models.Profile.AdminProfileDto
            {
                AdminName = admin.admin_name,
                Phone = admin.phone,
                Address = admin.address,
                Email = user.email
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("students")]
        public IHttpActionResult GetAllStudents()
        {
            var students = db.Students
                .Select(s => new
                {
                    s.stu_id,
                    s.stu_name,
                    s.mobile,
                    s.city,
                    s.State,
                    s.DOB,
                    s.Qualification,
                    s.Completion,
                    Email = db.Users
                              .Where(u => u.reference_Id == s.stu_id)
                              .Select(u => u.email)
                              .FirstOrDefault()
                })
                .ToList();

            return Ok(students);
        }

        [HttpGet]
        [Route("reports/coursereports")]
        public IHttpActionResult GetCourseLevelPassFail()
        {
            try
            {
                var data = db.courses
                    .Select(c => new
                    {
                        CourseName = c.course_name,
                        Levels = db.Levels
                            .Where(l => l.course_id == c.course_Id)
                            .Select(l => new
                            {
                                LevelNumber = l.level_number,
                                LevelName = l.level_name,
                                Passed = db.ExamAttempts
                                           .Count(e => e.course_id == c.course_Id
                                                       && e.level_number == l.level_number
                                                       && e.is_passed),
                                Failed = db.ExamAttempts
                                           .Count(e => e.course_id == c.course_Id
                                                       && e.level_number == l.level_number
                                                       && !e.is_passed)
                            })
                            .Where(lvl => (lvl.Passed + lvl.Failed) > 0) 
                            .ToList()
                    })
                    .Where(c => c.Levels.Any()) 
                    .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("exam-attempts")]
        public IHttpActionResult GetExamAttempts(int? studentId = null)
        {
            try
            {
                var attempts = (from a in db.ExamAttempts
                                join u in db.Users on a.user_id equals u.user_Id
                                join s in db.Students on u.reference_Id equals s.stu_id
                                join c in db.courses on a.course_id equals c.course_Id
                                join l in db.Levels on new { a.course_id, a.level_number } equals new { l.course_id, l.level_number }
                                orderby a.attempt_id descending
                                select new
                                {
                                    AttemptId = a.attempt_id,
                                    StudentId = s.stu_id,
                                    StudentName = s.stu_name,
                                    Email = u.email,
                                    CourseName = c.course_name,
                                    LevelNumber = a.level_number,
                                    TotalQuestions = a.total_questions,
                                    CorrectAnswers = a.correct_answers,
                                    Score = a.score,
                                    TotalTime = a.total_time,
                                    TimeTaken = a.time_taken,
                                    IsPassed = a.is_passed
                                }).ToList();

                if (studentId.HasValue)
                    attempts = attempts.Where(x => x.StudentId == studentId.Value).ToList();

                return Ok(attempts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
