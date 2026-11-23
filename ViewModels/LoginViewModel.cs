using System.ComponentModel.DataAnnotations;

namespace CLSWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public int MemberNumber { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}