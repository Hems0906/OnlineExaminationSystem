using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OnlineExaminationSystem.Models;
using System.Security.Cryptography;
using System.Text;


namespace OnlineExaminationSystem.Controllers.Home
{
    [RoutePrefix("api/home")]
    public class MainController : ApiController
    {
        OnlineExamSystemEntities4 db = new OnlineExamSystemEntities4();

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string hashedInput = HashPassword(enteredPassword);
            return hashedInput.Equals(storedHash);
        }


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
                    password = HashPassword(st.Password)/* st.Password*/,
                    role = "student",
                    reference_Id = stu.stu_id
                };

                db.Users.Add(user);
                db.SaveChanges();

                return Ok("Registration Successful!!");
            }
            catch (Exception ex)
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
                var user = db.Users.FirstOrDefault(u => u.email == login.Email);
                if (user == null)
                    return BadRequest("Invalid email or password!");

                bool isPasswordValid = false;

                if (user.password.Length > 40) 
                {
                    isPasswordValid = VerifyPassword(login.Password, user.password);
                }
                else
                {
                    if (user.password == login.Password)
                    {
                        user.password = HashPassword(login.Password);
                        db.SaveChanges();
                        isPasswordValid = true;
                    }
                }

                if (!isPasswordValid)
                    return BadRequest("Invalid email or password!");

                string userName = "";

                if (user.role == "Student")
                {
                    var student = db.Students.FirstOrDefault(s => s.stu_id == user.reference_Id);
                    if (student != null)
                        userName = student.stu_name;
                }
                else if (user.role == "Admin")
                {
                    var admin = db.Admins.FirstOrDefault(a => a.admin_id == user.reference_Id);
                    if (admin != null)
                        userName = admin.admin_name;
                }

                return Ok(new { userId = user.user_Id, role = user.role, email = user.email, stuId = user.reference_Id, userName = userName });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
