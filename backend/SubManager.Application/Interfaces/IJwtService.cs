using SubManager.Application.DTO.Account;
using SubManager.Domain.Entities;
using SubManager.Domain.IdentityEntities;
using System.Security.Claims;

namespace SubManager.Application.Interfaces
{
    public interface IJwtService
    {
        /// <summary>
        /// Create a jwt token
        /// </summary>
        /// <param name="user">The connected user</param>
        /// <returns>A jwt token</returns>
        public Task<TokenDto> CreateJwtTokenAsync(ApplicationUser user);

        /// <summary>
        /// Get a refresh token
        /// </summary>
        /// <param name="refreshToken">The wanted token</param>
        /// <returns>A refresh token</returns>
        public Task<RefreshToken> GetRefreshToken(string refreshToken);

        /// <summary>
        /// Revoke the specified refresh token
        /// </summary>
        /// <param name="user">The token's user</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RevokeRefreshTokenAsync(ApplicationUser user, string token);

        /// <summary>
        /// Check if an access token is valid
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <returns>The ClaimsPrincipal if true, null otherwise</returns>
        public ClaimsPrincipal? IsTokenValid(string token);

        /// <summary>
        /// Generate a refresh token
        /// </summary>
        /// <param name="user">The user who needs the token</param>
        /// <returns>A refresh token dto</returns>
        public Task<RefreshTokenDto> GenerateRefreshTokenAsync(ApplicationUser user);

        /// <summary>
        /// Revoke the old token and create a new one
        /// </summary>
        /// <param name="oldToken">The old token</param>
        /// <param name="user">The user who needs the token</param>
        /// <returns>A new refresh token dto</returns>
        public Task<RefreshTokenDto> RotateRefreshTokenAsync(string oldToken, ApplicationUser user);
    }
}
