using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubManager.Application.DTO.Account;
using SubManager.Application.Interfaces;
using SubManager.Domain.Entities;
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

                Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshToken.Expire
                });

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

            Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.Expire
            });

            return Ok(token);
        }

        [HttpGet("api/logout")]
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

        [HttpPost("api/refresh-token")]
        public async Task<ActionResult<TokenDto>> RefreshToken()
        {
            var token = Request.Cookies["refreshToken"];
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("No refresh token found");
            }

            if (user == null)
            {
                return BadRequest("No user found");
            }

            var newToken = await _jwtService.RotateRefreshTokenAsync(token, user);
            var accessToken = await _jwtService.CreateJwtTokenAsync(user);

            Response.Cookies.Append("refreshToken", newToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newToken.Expire
            });

            return Ok(accessToken);
        }
    }
}
