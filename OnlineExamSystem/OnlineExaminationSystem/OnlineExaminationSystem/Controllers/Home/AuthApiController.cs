using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Web.Http;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Controllers.Home
{
    [RoutePrefix("api/auth")]
    public class AuthApiController : ApiController
    {
        OnlineExamSystemEntities2 db = new OnlineExamSystemEntities2();

        [HttpGet]
        [Route("exists")]
        public IHttpActionResult Exists([FromUri] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("email is required");
            var normalized = email.Trim().ToLower();
            var user = db.Users.FirstOrDefault(u => u.email.Trim().ToLower() == normalized);
            return Ok(new { email = normalized, found = user != null });
        }

        [HttpPost]
        [Route("forgotpassword")]
        public IHttpActionResult ForgotPassword([FromBody] Models.Home.ForgotPasswordRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email))
                return BadRequest("Email is required");

            var normalizedEmail = req.Email.Trim().ToLower();
            var user = db.Users.FirstOrDefault(u => u.email.Trim().ToLower() == normalizedEmail);

            if (user == null) return NotFound();

            if (user.role != null && user.role.ToLower() != "student")
            {
                return BadRequest("Admins are not allowed to reset password");
            }

            string otp = new Random().Next(100000, 999999).ToString();

            db.UserOTPs.Add(new UserOTP
            {
                user_id = user.user_Id,
                otp = otp,
                created_at = DateTime.Now,
                is_used = false
            });
            db.SaveChanges();

            SendEmail(req.Email, "Password Reset OTP", $"Your OTP is: {otp}");

            return Ok("OTP sent to your email");
        }

        [HttpPost]
        [Route("verifyotp")]
        public IHttpActionResult VerifyOtp([FromBody] Models.Home.VerifyOtpRequest req)
        {
            var normalizedEmail = req.Email.Trim().ToLower();
            var user = db.Users.FirstOrDefault(u => u.email.Trim().ToLower() == normalizedEmail);

            if (user == null) return NotFound();

            var otpRecord = db.UserOTPs
                .Where(o => o.user_id == user.user_Id && o.otp == req.OTP && o.is_used == false)
                .OrderByDescending(o => o.created_at)
                .FirstOrDefault();

            if (otpRecord != null && otpRecord.created_at != null &&
                (DateTime.Now - otpRecord.created_at.Value).TotalMinutes <= 10)
            {
                otpRecord.is_used = true;
                db.SaveChanges();
                return Ok("OTP Verified");
            }
            return BadRequest("Invalid or expired OTP");
        }

        [HttpPost]
        [Route("resetpassword")]
        public IHttpActionResult ResetPassword([FromBody] Models.Home.ResetPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest("Email and new password are required");

            string normalizedEmail = req.Email.Trim().ToLower();
            var user = db.Users.FirstOrDefault(u => u.email.Trim().ToLower() == normalizedEmail);

            if (user == null) return NotFound();

            if (user.role != null && user.role.ToLower() != "student")
            {
                return BadRequest("Invalid Email");
            }

            user.password = req.NewPassword;  
            db.SaveChanges();

            return Ok("Password updated successfully");
        }

        private void SendEmail(string to, string subject, string body)
        {
            var from = "infiniteprojecttest@gmail.com";
            var password = "punt gpsv ogqm mzjd";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from, password),
                EnableSsl = true
            };

            client.Send(new MailMessage(from, to, subject, body));
        }
    }
}
