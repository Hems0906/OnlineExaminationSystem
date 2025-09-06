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
        OnlineExamSystemEntities1 db = new OnlineExamSystemEntities1();

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

                    int nextLevel = (progress == null || !progress.highest_level_passed.HasValue)? 1: progress.highest_level_passed.Value + 1;

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
    }
}
