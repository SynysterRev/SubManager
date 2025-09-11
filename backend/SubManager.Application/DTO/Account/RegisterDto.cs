using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SubManager.Application.DTO.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "A email is required")]
        [EmailAddress(ErrorMessage = "Eamil should be in a proper format")]
        [Remote(action: "IsEmailAvailable", controller: "Account", ErrorMessage = "This email is already use")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
