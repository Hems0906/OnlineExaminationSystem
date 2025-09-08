using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_OES.Services
{
    public class CaptchaService
    {
        private const string SessionKey = "CaptchaCode";
        public string GenerateCaptcha(int length = 6)
        {
            var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            var captcha = new char[length];

            for (int i = 0; i < length; i++)
            {
                captcha[i] = chars[random.Next(chars.Length)];
            }

            HttpContext.Current.Session[SessionKey] = new string(captcha);
            return new string(captcha);
        }
        public bool ValidateCaptcha(string input)
        {
            var stored = HttpContext.Current.Session[SessionKey] as string;
            return stored != null && stored.Equals(input, StringComparison.OrdinalIgnoreCase);
        }
    }
}