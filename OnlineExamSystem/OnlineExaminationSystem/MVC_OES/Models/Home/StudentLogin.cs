using System.ComponentModel.DataAnnotations;

namespace MVC_OES.Models.Home
{
    public class StudentLogin
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
