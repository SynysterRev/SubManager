using System.ComponentModel.DataAnnotations;

namespace SubManager.Application.DTO.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "A email is required")]
        [EmailAddress(ErrorMessage = "Eamil should be in a proper format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
