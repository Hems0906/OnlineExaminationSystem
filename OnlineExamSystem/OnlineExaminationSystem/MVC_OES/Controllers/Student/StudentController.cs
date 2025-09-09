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

        public async Task<ActionResult> Dashboard()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            ViewBag.UserId = userId;
            ViewBag.UserEmail = Session["UserEmail"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync($"dashboard/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic stats = JsonConvert.DeserializeObject(json);
                    ViewBag.Stats = stats;
                }
                else
                {
                    ViewBag.Stats = null;
                }
            }

            return View();
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<ActionResult> Instructions(int courseId, int levelNumber)
        {
            InstructionModel instruction = null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44377/api/exam/");
                var response = await client.GetAsync($"instructions/{courseId}/{levelNumber}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    instruction = JsonConvert.DeserializeObject<InstructionModel>(json);
                }
                else
                {
                    ViewBag.Error = "Could not load instructions.";
                }
            }

            return View(instruction);
        }


        public async Task<ActionResult> StartExam(int courseId, int levelNumber)
        {
            var userId = (int)Session["UserId"];
            StartExamModel examModel = null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44377/api/exam/");

                var response = await client.GetAsync($"start/{userId}/{courseId}/{levelNumber}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    examModel = JsonConvert.DeserializeObject<StartExamModel>(json);
                }
                else
                {
                    ViewBag.Error = "Could not load exam questions.";
                }
            }

            return View(examModel);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitExam(SubmitExamRequest request)
        {
            if (request == null)
                return new HttpStatusCodeResult(400, "Invalid request");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44377/api/exam/");
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync("submit", content);
                if (response.IsSuccessStatusCode)
                {
                    var respJson = await response.Content.ReadAsStringAsync();
                    var report = JsonConvert.DeserializeObject<ExamReportViewModel>(respJson);

                    report.courseId = request.courseId;
                    return Json(report, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return new HttpStatusCodeResult((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
        }

        [HttpGet]
        public ActionResult ReportFromSubmit(string reportJson)
        {
            if (string.IsNullOrEmpty(reportJson))
                return RedirectToAction("Courses");

            var report = JsonConvert.DeserializeObject<ExamReportViewModel>(reportJson);

            ViewBag.courseId = report.courseId;
            return View(report);
        }

        [HttpGet]
        public async Task<ActionResult> Index(int userId)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;

            List<ExamResultViewModel> results = new List<ExamResultViewModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync($"getresults/{userId}");
                var raw = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    results = JsonConvert.DeserializeObject<List<ExamResultViewModel>>(raw);
                }
                else
                {
                    ViewBag.Error = "Unable to fetch results.";
                }
            }

            return View(results);
        }

        [HttpGet]
        public async Task<ActionResult> SendCompletionEmail(int userId, int courseId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44377/api/courses/");

                    var response = await client.GetAsync($"completedcourse/{userId}/{courseId}");

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Email sent successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to send email." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}