using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_OES.Models.Home;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using MVC_OES.Services;
using Newtonsoft.Json;

namespace MVC_OES.Controllers.Home
{
    public class MainController : Controller
    {
        private readonly string baseUrl = "https://localhost:44377/api/home/";
        private readonly string authApiUrl = "https://localhost:44377/api/auth/";
        private readonly CaptchaService _captchaService = new CaptchaService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.CaptchaCode = _captchaService.GenerateCaptcha();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(StudentRegister model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!_captchaService.ValidateCaptcha(model.CaptchaInput))
            {
                ViewBag.ErrorMessage = "Invalid captcha.";
                ViewBag.CaptchaCode = _captchaService.GenerateCaptcha();
                return View(model);
            }

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
                        var data = await response.Content.ReadAsAsync<Models.Home.LoginResponse>();

                        int stuID = data.stuId;
                        int userId = data.userId;
                        string role = data.role;
                        string email = data.email;
                        string name = data.userName;

                        Session["StuId"] = stuID;
                        Session["UserId"] = userId;
                        Session["UserEmail"] = email;
                        Session["UserRole"] = role;
                        Session["UserName"] = name;

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

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ErrorMessage = "Please enter your email";
                return View();
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

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

        [HttpGet]
        public ActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VerifyOtp(string otp)
        {
            string email = Session["ResetEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.ErrorMessage = "Session expired. Please try Forgot Password again.";
                return RedirectToAction("ForgotPassword");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

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

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match";
                return View();
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

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
                    Session.Remove("ResetEmail"); 
                    TempData["SuccessMessage"] = "Password updated successfully!";
                    return RedirectToAction("Login");
                }

                ViewBag.ErrorMessage = await ExtractApiError(response);
                return View();
            }
        }

        private async Task<string> ExtractApiError(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                    return "An unknown error occurred.";

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