using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubManager.Application.DTO.Account;
using SubManager.Application.Interfaces;
using SubManager.Domain.IdentityEntities;
using System.Security.Claims;

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
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user);

                SetCookie(Response, refreshToken);

                return Ok(token);
            }
            else
            {
                return BadRequest(new
                {
                    message = "Registration failed",
                    errors = result.Errors.Select(e => e.Description).ToList()
                });
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
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user);

            SetCookie(Response, refreshToken);

            return Ok(token);
        }

        [HttpPost("api/logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            var token = Request.Cookies["refreshToken"];
            var user = await _userManager.GetUserAsync(HttpContext.User);
            
            var nameIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("No refresh token found");
            }

            if (user == null)
            {
                return BadRequest("No user found");
            }

            await _jwtService.RevokeRefreshTokenAsync(user, token);
            Response.Cookies.Delete("refreshToken");

            return NoContent();
        }

        [HttpGet("api/email")]
        public async Task<ActionResult> IsEmailAvailable(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return Ok(user == null);
        }

        [AllowAnonymous]
        [HttpPost("api/refresh-token")]
        public async Task<ActionResult<TokenDto>> RefreshToken()
        {
            var token = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("No refresh token found");
            }

            var refreshToken = await _jwtService.GetRefreshToken(token);

            if (refreshToken == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            var user = refreshToken.User;

            var newToken = await _jwtService.RotateRefreshTokenAsync(token, user);
            var accessToken = await _jwtService.CreateJwtTokenAsync(user);

            SetCookie(Response, newToken);

            return Ok(accessToken);
        }

        private void SetCookie(HttpResponse response, RefreshTokenDto refreshToken)
        {
            response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = refreshToken.Expire,
                IsEssential = true
            });
        }
    }
}
