using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Filters;
using System.Threading.Tasks;
using System.Net.Http;
using MVC_OES.Models.Courses;
using System.Net;

namespace MVC_OES.Controllers.Admin
{
    [AuthorizeRoles("admin")]
    public class AdminController : Controller
    {
        private readonly string baseUrl = "https://localhost:44377/api/admin/";

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Courses()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.GetAsync("getcourses");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<List<CourseViewModel>>();
                    ViewBag.Courses = data;
                }
                else
                {
                    ViewBag.Courses = null;
                    ViewBag.ErrorMessage = "Failed to fetch courses.";
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddCourse(AddCourseModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.PostAsJsonAsync("addcourse", model);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<dynamic>();
                    int courseId = result.courseId;
                    return RedirectToAction("AddLevels", new { courseId = courseId });
                }
                else
                {
                    ViewBag.ErrorMessage = await response.Content.ReadAsStringAsync();
                    return View(model);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> ToggleStatus(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                var response = await client.PutAsync($"toggle/{id}", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<dynamic>();
                    return Json(new { success = true, message = (string)result.message });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update status." });
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditCourse(int courseId)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.GetAsync("getcourses");

                if (response.IsSuccessStatusCode)
                {
                    var courses = await response.Content.ReadAsAsync<List<CourseViewModel>>();
                    var course = courses.FirstOrDefault(c => c.CourseId == courseId);

                    if (course == null)
                        return HttpNotFound();

                    var model = new AddCourseModel
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName
                    };

                    return View(model);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to fetch course details.";
                    return RedirectToAction("Courses");
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditCourse(AddCourseModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.PutAsJsonAsync($"edit/{model.CourseId}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Course updated successfully!";
                    return RedirectToAction("Courses");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = errorMessage;
                    return View(model);
                }
            }
        }



        [HttpGet]
        public ActionResult AddLevels(int courseId)
        {
            ViewBag.CourseId = courseId;

            var model = new List<Models.Levels.LevelViewModel>
            {
                new Models.Levels.LevelViewModel { LevelNumber = 1 },
                new Models.Levels.LevelViewModel { LevelNumber = 2 },
                new Models.Levels.LevelViewModel { LevelNumber = 3 }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddLevels(int courseId, List<Models.Levels.LevelViewModel> models)
        {
            if (!ModelState.IsValid)
                return View(models);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.PostAsJsonAsync($"AddLevels/{courseId}", models);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Levels added successfully!";
                    return RedirectToAction("Levels", new { courseId = courseId });
                }
                else
                {
                    ViewBag.ErrorMessage = await response.Content.ReadAsStringAsync();
                    return View(models);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Levels(int courseId)
        {
            var levels = new List<Models.Levels.LevelViewModel>();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.GetAsync($"getlevels/{courseId}");

                if (response.IsSuccessStatusCode)
                {
                    levels = await response.Content.ReadAsAsync<List<Models.Levels.LevelViewModel>>();
                }
                else
                {
                    var raw = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(raw);
                }
            }

            ViewBag.CourseId = courseId;
            return View(levels);
        }

        //[HttpGet]
        //public async Task<ActionResult> EditLevel(int courseId, int levelNumber)
        //{
        //    Models.Levels.LevelViewModel level = null;

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(baseUrl);
        //        var response = await client.GetAsync($"getlevel/{courseId}/{levelNumber}");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            level = await response.Content.ReadAsAsync<Models.Levels.LevelViewModel>();
        //        }
        //    }

        //    if (level == null)
        //        return RedirectToAction("Levels", new { courseId });

        //    level.CourseId = courseId;
        //    level.LevelNumber = levelNumber;

        //    return View(level);
        //}

        [HttpGet]
        public async Task<ActionResult> EditLevel(int courseId, int levelNumber)
        {
            Models.Levels.LevelViewModel level = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;

                // Call the API to get the level data
                var response = await client.GetAsync($"getlevel/{courseId}/{levelNumber}");

                if (response.IsSuccessStatusCode)
                {
                    level = await response.Content.ReadAsAsync<Models.Levels.LevelViewModel>();
                }
            }

            if (level == null)
            {
                TempData["ErrorMessage"] = "Level not found!";
                return RedirectToAction("Levels", new { courseId });
            }

            return View(level);
        }


        //[HttpPost]
        //public async Task<ActionResult> EditLevel(Models.Levels.LevelViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(baseUrl);
        //        var response = await client.PutAsJsonAsync($"editlevel/{model.CourseId}/{model.LevelNumber}", model);

        //        if (response.IsSuccessStatusCode)
        //            return RedirectToAction("Levels", new { courseId = model.CourseId });
        //    }

        //    ViewBag.ErrorMessage = "Failed to update level.";
        //    return View(model);
        //}

        [HttpPost]
        public async Task<ActionResult> EditLevel(Models.Levels.LevelViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback += (s, c, ch, e) => true;

                // Call API to update the level
                var response = await client.PutAsJsonAsync($"editlevel/{model.CourseId}/{model.LevelNumber}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Level updated successfully!";
                    return RedirectToAction("Levels", new { courseId = model.CourseId });
                }
            }

            ViewBag.ErrorMessage = "Failed to update level.";
            return View(model);
        }
    }
}