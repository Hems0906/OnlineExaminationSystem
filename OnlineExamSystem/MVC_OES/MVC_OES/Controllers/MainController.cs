using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_OES.Models.Home;

namespace MVC_OES.Controllers
{
    public class MainController : Controller
    {
        // Change this to your Web API URL and port
        private readonly string baseUrl = "https://localhost:44330/api/home/";

        // GET: Main/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Main/Register
        [HttpPost]
        public async Task<ActionResult> Register(StudentRegister model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                try
                {
                    var response = await client.PostAsJsonAsync("register", model);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Registration successful! Please login.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = errorMessage;
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error connecting to API: " + ex.Message;
                    return View(model);
                }
            }
        }

        // GET: Main/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(StudentLogin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                try
                {
                    var response = await client.PostAsJsonAsync("login", model);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<dynamic>();

                        string role = data.role;
                        string email = data.email;

                        // Store in session
                        Session["UserEmail"] = email;
                        Session["UserRole"] = role;

                        if (role == "admin")
                            return RedirectToAction("Dashboard", "Admin");
                        else
                            return RedirectToAction("Dashboard", "Student");
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = errorMessage;
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error connecting to API: " + ex.Message;
                    return View(model);
                }
            }
        }
    }
}
