using SubManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Domain.Repositories
{
    public interface ISubscriptionRepository
    {
        /// <summary>
        /// Get all subscriptions in the database
        /// </summary>
        /// <returns>A list of all subscriptions</returns>
        public IQueryable<Subscription> GetAllSubscriptions();

        /// <summary>
        /// Get all subscriptions for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A list of all subscriptions for the user</returns>
        public IQueryable<Subscription> GetAllSubscriptionsByUser(Guid userId);

        /// <summary>
        /// Get subscription by its ID
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription</param>
        /// <returns>The subscription if found, null otherwise</returns>
        public Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId);

        /// <summary>
        /// Add a new subscription in the database
        /// </summary>
        /// <param name="subscription">The new subscription to add</param>
        /// <returns>The added subscription</returns>
        public Task<Subscription> AddSubscriptionAsync(Subscription subscription);

        /// <summary>
        /// Update a subscription
        /// </summary>
        public Task SaveChangesAsync();

        /// <summary>
        /// Delete a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription to delete</param>
        public Task DeleteSubscriptionAsync(Subscription subscription);
    }
}
