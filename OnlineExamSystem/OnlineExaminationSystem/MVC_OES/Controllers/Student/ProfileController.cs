using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Filters;
using MVC_OES.Models.Profile;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Security;

namespace MVC_OES.Controllers.Student
{
    [AuthorizeRoles("student")]
    public class ProfileController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:44377/api/";

        public async Task<ActionResult> StudentProfileDetails()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            StudentProfileDto student = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                var response = await client.GetAsync($"profile/get?id={userId}");

                if (response.IsSuccessStatusCode)
                {
                    student = await response.Content.ReadAsAsync<StudentProfileDto>();
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to fetch profile.";
                    return RedirectToAction("Login", "Main");
                }
            }

            return View("StudentProfileDetails", student);
        }

        [HttpGet]
        public async Task<ActionResult> StudentProfileEdit()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            StudentProfileDto student = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                var response = await client.GetAsync($"profile/get?id={userId}");

                if (response.IsSuccessStatusCode)
                {
                    student = await response.Content.ReadAsAsync<StudentProfileDto>();
                }
            }

            return View("StudentProfileEdit", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentProfileEdit(StudentProfileDto model)
        {
            if (!ModelState.IsValid)
                return View("StudentProfileEdit", model);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                int userId = Convert.ToInt32(Session["UserId"]);

                var response = await client.PutAsJsonAsync($"profile/update?id={userId}", model);

                if (response.IsSuccessStatusCode)
                {
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    TempData["SuccessMessage"] = "Profile updated successfully. Please login again.";
                    return RedirectToAction("Logout", "Main");
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", errorMsg);
                    return View("StudentProfileEdit", model);
                }
            }
        }
    }
}