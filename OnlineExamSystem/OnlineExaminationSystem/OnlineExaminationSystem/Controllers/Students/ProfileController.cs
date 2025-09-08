using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Controllers.Students
{
    [RoutePrefix("api/profile")]
    public class ProfileController : ApiController
    {
        OnlineExamSystemEntities2 db = new OnlineExamSystemEntities2();

        [HttpGet]
        [Route("get")]
        public IHttpActionResult GetProfile(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.user_Id == id && u.role == "Student");
            if (user == null)
                return Content(HttpStatusCode.NotFound, "User not found");

            var student = db.Students.FirstOrDefault(s => s.stu_id == user.reference_Id);
            if (student == null)
                return Content(HttpStatusCode.NotFound, "Student profile not found");

            var dto = new Models.Profile.StudentProfileDto
            {
                stu_name = student.stu_name,
                mobile = student.mobile,
                city = student.city,
                State = student.State,
                DOB = student.DOB,
                Qualification = student.Qualification,
                Completion = student.Completion,
                email = user.email
            };

            return Ok(dto);
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateProfile(Models.Profile.StudentProfileDto model, int id)
        {
            if (model == null)
                return BadRequest("Invalid data");

            var user = db.Users.FirstOrDefault(u => u.user_Id == id && u.role == "Student");
            if (user == null)
                return Content(HttpStatusCode.NotFound, "User not found");

            var student = db.Students.FirstOrDefault(s => s.stu_id == user.reference_Id);
            if (student == null)
                return Content(HttpStatusCode.NotFound, "Student profile not found");

            user.email = model.email;

            student.stu_name = model.stu_name;
            student.mobile = model.mobile;
            student.city = model.city;
            student.State = model.State;
            student.DOB = model.DOB;
            student.Qualification = model.Qualification;
            student.Completion = model.Completion;

            db.SaveChanges();

            return Ok("Profile updated successfully");
        }
    }
}
