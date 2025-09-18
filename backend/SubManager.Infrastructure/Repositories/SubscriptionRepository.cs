using Microsoft.EntityFrameworkCore;
using SubManager.Domain.Entities;
using SubManager.Domain.Repositories;
using SubManager.Infrastructure.DatabaseContext;

namespace SubManager.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription> AddSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task DeleteSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Subscription> GetAllSubscriptions()
        {
            return _context.Subscriptions;
        }

        public IQueryable<Subscription> GetAllSubscriptionsByUser(Guid userId)
        {
            return _context.Subscriptions.Where(s => s.UserId == userId).OrderBy(s => s.Id);
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId, Guid userId)
        {
            return await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == subscriptionId && s.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
