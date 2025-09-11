using SubManager.Application.DTO.Account;
using SubManager.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Check if an access token is valid
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsTokenValid(string token);

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
