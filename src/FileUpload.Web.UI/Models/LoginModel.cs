using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
