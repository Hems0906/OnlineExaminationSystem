using System.Web.Mvc;
using System.Web.Security;
using mvcfinal.Models;

namespace mvcfinal.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Simple hardcoded users for demo
                if ((model.Email == "admin@test.com" && model.Password == "admin") ||
                    (model.Email == "inst@test.com" && model.Password == "inst") ||
                    (model.Email == "student@test.com" && model.Password == "student"))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    if (model.Email.Contains("admin")) return RedirectToAction("Index", "Admin");
                    if (model.Email.Contains("inst")) return RedirectToAction("Index", "Institution");
                    return RedirectToAction("Index", "Student");
                }

                ModelState.AddModelError("", "Invalid credentials");
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
