using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubManager.Application.DTO.Account;
using SubManager.Application.Interfaces;
using SubManager.Domain.IdentityEntities;

namespace SubManager.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<ApplicationRole> roleManager, 
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost("api/register")]
        public async Task<ActionResult<TokenDto>> Register(RegisterDto registerDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);

                var token = await _jwtService.CreateJwtTokenAsync(user);

                return Ok(token);
            }
            else
            {
                string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
                return Problem(errorMessage);
            }
        }

        [HttpPost("api/login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid email or password");

            }
            ApplicationUser? user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            var token = await _jwtService.CreateJwtTokenAsync(user);

            return Ok(token);
        }

        [HttpGet("api/logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        [HttpGet("api/email")]
        public async Task<ActionResult> IsEmailAvailable(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return Ok(user == null);
        }

        [HttpPost("api/refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshToken)
        {

        }

    }
}
