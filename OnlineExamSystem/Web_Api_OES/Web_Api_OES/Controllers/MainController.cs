using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_Api_OES.Models;

namespace Web_Api_OES.Controllers
{
    [RoutePrefix("api/home")]
    public class MainController : ApiController
    {
        OnlineExamSystemEntities db = new OnlineExamSystemEntities();

        [HttpPost]
        [Route("register")]
        public IHttpActionResult register([FromBody] Models.Home.StudentRegister st)
        {
            try
            {
                if (db.Users.Any(u => u.email == st.Email))
                {
                    return BadRequest("Email already registered!");
                }

                var stu = new Student
                {
                    stu_name = st.StuName,
                    mobile = st.Mobile,
                    city = st.City,
                    State = st.State,
                    DOB = st.DOB,
                    Qualification = st.Qualification,
                    Completion = st.Completion
                };

                db.Students.Add(stu);
                db.SaveChanges();

                var user = new User
                {
                    email = st.Email,
                    password = st.Password,
                    role = "student",
                    reference_Id = stu.stu_id
                };

                db.Users.Add(user);
                db.SaveChanges();

                return Ok("Registration Successful!!");
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] Models.Home.StudentLogin login)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.email == login.Email && u.password == login.Password);
                if (user == null)
                    return BadRequest("Invalid email or password!");

                return Ok(new { userId = user.user_Id, role = user.role, email = user.email });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
