using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Models.Home;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace MVC_OES.Controllers.Home
{
    public class MainController : Controller
    {
        private readonly string baseUrl = "https://localhost:44377/api/home/";
        private readonly string authApiUrl = "https://localhost:44377/api/auth/";

        public ActionResult Index()
        {
            return View();
        }

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

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Main");
        }
        // Forgot Password (Enter Email)
        public ActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ErrorMessage = "Please enter your email";
                return View();
            }

            using (var client = new HttpClient())
            {
                var request = new { Email = email.Trim().ToLower() };
                var response = await client.PostAsJsonAsync(authApiUrl + "forgotpassword", request);

                if (response.IsSuccessStatusCode)
                {
                    Session["ResetEmail"] = email.Trim().ToLower();
                    return RedirectToAction("VerifyOtp");
                }

                ViewBag.ErrorMessage = await ExtractApiError(response);
                return View();
            }
        }

        // Verify OTP
        public ActionResult VerifyOtp() => View();

        [HttpPost]
        public async Task<ActionResult> VerifyOtp(string otp)
        {
            string email = Session["ResetEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Session expired. Please try Forgot Password again.";
                return RedirectToAction("ForgotPassword");
            }

            using (var client = new HttpClient())
            {
                var req = new { Email = email, OTP = otp };
                var response = await client.PostAsJsonAsync(authApiUrl + "verifyotp", req);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ResetPassword");
                }

                ViewBag.ErrorMessage = await ExtractApiError(response);
                return View();
            }
        }

        // Reset Password
        public ActionResult ResetPassword() => View();

        [HttpPost]
        public async Task<ActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match";
                return View();
            }

            string email = Session["ResetEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Session expired. Please try Forgot Password again.";
                return RedirectToAction("ForgotPassword");
            }

            using (var client = new HttpClient())
            {
                var req = new { Email = email, NewPassword = newPassword };
                var response = await client.PostAsJsonAsync(authApiUrl + "resetpassword", req);

                if (response.IsSuccessStatusCode)
                {
                    Session.Remove("ResetEmail"); // clear session
                    TempData["SuccessMessage"] = "Password updated successfully!";
                    return RedirectToAction("Login");
                }

                ViewBag.ErrorMessage = await ExtractApiError(response);
                return View();
            }
        }

        // 🔹 Helper to extract API error message
        private async Task<string> ExtractApiError(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                    return "An unknown error occurred.";

                // Try parse JSON { "Message": "..." }
                dynamic errorObj = JsonConvert.DeserializeObject(content);
                return errorObj?.Message ?? content;
            }
            catch
            {
                return "An unknown error occurred.";
            }
        }
    }
}