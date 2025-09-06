using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Filters;
using System.Threading.Tasks;
using System.Net.Http;
using MVC_OES.Models.Student;
using Newtonsoft.Json;
using System.Net;

namespace MVC_OES.Controllers.Student
{
    [AuthorizeRoles("student")]
    public class StudentController : Controller
    {

        private readonly string baseUrl = "https://localhost:44377/api/courses/";

        public ActionResult Dashboard()
        {
            //if (Session["UserRole"]?.ToString() != "student")
            //    return RedirectToAction("Login", "Main");

            ViewBag.UserId = Session["UserId"];
            ViewBag.UserEmail = Session["UserEmail"];

            return View();
        }

        public async Task<ActionResult> Courses(int userId)
        {
            List<CourseModel> courses = new List<CourseModel>();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.GetAsync($"getcourses/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    courses = JsonConvert.DeserializeObject<List<CourseModel>>(json);
                }
                else
                {
                    ViewBag.Error = "Could not load courses from API.";
                }
            }

            return View(courses);
        }
    }
}