using Microsoft.EntityFrameworkCore;
using SubManager.Domain.Entities;
using SubManager.Domain.Repositories;
using SubManager.Infrastructure.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Infrastructure.Repositories
{
    public class JwtRepository : IJwtRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public JwtRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRefreshToken(RefreshToken token)
        {
            _dbContext.RefreshTokens.Add(token);
        }

        public async Task<RefreshToken?> GetRefreshToken(string token, Guid userId)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token && t.UserId == userId);
        }

        public async Task<RefreshToken?> GetRefreshToken(string token)
        {
            return await _dbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
