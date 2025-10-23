using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SubManager.Application.DTO.Account;
using SubManager.Application.Interfaces;
using SubManager.Domain.Entities;
using SubManager.Domain.IdentityEntities;
using SubManager.Domain.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SubManager.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtRepository _jwtRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<JwtService> logger, IJwtRepository jwtRepository)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _jwtRepository = jwtRepository;
        }

        public async Task<TokenDto> CreateJwtTokenAsync(ApplicationUser user)
        {
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("Secret key for JWT token is not set in configuration.");
            }

            var issuer = _configuration["Jwt:Issuer"];
            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("Issuer for JWT token is not set in configuration.");
            }

            var audience = _configuration["Jwt:Audience"];
            //if (string.IsNullOrEmpty(issuer))
            //{
            //    throw new InvalidOperationException("Audience for JWT token is not set in configuration.");
            //}

            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            var roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                // unique id for token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Issued at (date and time of token generation), meaning when the token is created
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                //unique name identifier of the user (email), optional
                new Claim(ClaimTypes.NameIdentifier, user.Email!),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires:
                expiration, signingCredentials: signingCredentials);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string token = handler.WriteToken(tokenGenerator);

            _logger.Log(LogLevel.Information, $"New token generated for user {user.Id}, {user.Email!}");
            return new TokenDto
            {
                Email = user.Email!,
                Expiration = expiration,
                Token = token,
                IsPremium = user.IsPremium,
            };
        }

        public ClaimsPrincipal? IsTokenValid(string token)
        {
            var secretKey = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key for JWT token is not set in configuration.");
            }

            var issuer = _configuration["Jwt:Issuer"];
            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("Issuer for JWT token is not set in configuration.");
            }

            var audience = _configuration["Jwt:Audience"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null; // Invalid token
            }
        }

        public async Task<RefreshTokenDto> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = GenerateRefreshToken(user);
            _jwtRepository.AddRefreshToken(refreshToken);
            await _jwtRepository.SaveChangesAsync();

            _logger.LogInformation($"Generate new refresh token for user {user.Id}, {user.Email}");

            return new RefreshTokenDto
            {
                Token = refreshToken.Token,
                Expire = refreshToken.Expires,
                IsRevoked = false
            };
        }

        private RefreshToken GenerateRefreshToken(ApplicationUser user)
        {
            var expirationTime = _configuration["RefreshToken:EXPIRATION_DAYS"];

            if (string.IsNullOrEmpty(expirationTime))
            {
                throw new InvalidOperationException("Expiration for refresh token is not set in configuration.");
            }

            Byte[] bytes = new byte[64];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            string token = Convert.ToBase64String(bytes);
            DateTime expiration = DateTime.UtcNow.AddDays(Convert.ToDouble(expirationTime));

            return new RefreshToken
            {
                Token = token,
                UserId = user.Id,
                IsRevoked = false,
                Expires = expiration,
            };
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var existingToken = await _jwtRepository.GetRefreshToken(refreshToken);

            if (existingToken == null || existingToken.IsRevoked || existingToken.Expires <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token invalid");
            }

            return existingToken;
        }

        private async Task<RefreshToken> GetRefreshToken(string refreshToken, Guid userId)
        {
            var existingToken = await _jwtRepository.GetRefreshToken(refreshToken, userId);

            if (existingToken == null || existingToken.IsRevoked || existingToken.Expires <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token invalid");
            }

            return existingToken;
        }

        public async Task<RefreshTokenDto> RotateRefreshTokenAsync(string oldToken, ApplicationUser user)
        {
            var existingToken = await GetRefreshToken(oldToken, user.Id);

            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;

            var newToken = GenerateRefreshToken(user);

            _jwtRepository.AddRefreshToken(newToken);
            await _jwtRepository.SaveChangesAsync();

            return new RefreshTokenDto
            {
                Token = newToken.Token,
                Expire = newToken.Expires,
                IsRevoked = false
            };
        }

        public async Task RevokeRefreshTokenAsync(ApplicationUser user, string token)
        {
            var existingToken = await GetRefreshToken(token, user.Id);

            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
            await _jwtRepository.SaveChangesAsync();
        }
    }
}
