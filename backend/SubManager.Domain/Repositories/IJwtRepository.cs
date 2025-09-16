using SubManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Domain.Repositories
{
    public interface IJwtRepository
    {
        /// <summary>
        /// Add a new refresh token in the database
        /// </summary>
        /// <param name="token">The token to add</param>
        public void AddRefreshToken(RefreshToken token);

        /// <summary>
        /// Get a refresh token by its token
        /// </summary>
        /// <param name="refreshTokenId">The token of the refresh token</param>
        /// <param name="userId">The ID of the token's user</param>
        /// <returns>The refresh token if found, null otherwise</returns>
        public Task<RefreshToken?> GetRefreshToken(string token, Guid userId);

        /// <summary>
        /// Update an existing refresh token
        /// </summary>
        /// <param name="refreshTokenId">The ID of the token to update</param>
        public Task SaveChangesAsync();
    }
}
