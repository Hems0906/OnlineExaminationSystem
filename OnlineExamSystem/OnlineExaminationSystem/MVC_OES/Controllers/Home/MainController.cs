using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Models.Home;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace MVC_OES.Controllers.Home
{
    public class MainController : Controller
    {
        private readonly string baseUrl = "https://localhost:44377/api/home/";

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

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

                        int stuID = data.stuId;
                        int userId = data.userId;
                        string role = data.role;
                        string email = data.email;

                        Session["StuId"] = stuID;
                        Session["UserId"] = userId;
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