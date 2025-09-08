using MVC_OES.Models.Profile;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using MVC_OES.Filters;

namespace MVC_OES.Controllers.Student
{
    [AuthorizeRoles("student")]
    public class ProfileController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:44377/api/"; // change to your API base URL

        // GET: Profile Details
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
                    return RedirectToAction("Login", "Account");
                }
            }

            return View("StudentProfileDetails", student); // 👈 View name changed
        }

        // GET: Edit Profile
        [HttpGet]
        //[ActionName("Edit")]
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

            return View("StudentProfileEdit", student); // 👈 View name changed
        }

        // POST: Edit Profile
        [HttpPost]
        //[ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentProfileEdit(StudentProfileDto model)
        {
            if (!ModelState.IsValid)
                return View("StudentProfileEdit", model); // 👈 View name changed


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                int userId = Convert.ToInt32(Session["UserId"]);  // ✅ get logged-in userId

                var response = await client.PutAsJsonAsync($"profile/update?userId={userId}", model);

                if (response.IsSuccessStatusCode)
                {
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    TempData["SuccessMessage"] = "Successfully changed the details. Please login again.";

                    return RedirectToAction("Logout", "Main");
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", errorMsg);
                    return View("StudentProfileEdit", model); // 👈 View name changed
                }
            }
        }
    }
}