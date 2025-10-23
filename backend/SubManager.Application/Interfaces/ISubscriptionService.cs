using SubManager.Application.DTO.Pagination;
using SubManager.Application.DTO.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Application.Interfaces
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Create a new subscription
        /// </summary>
        /// <param name="subscriptionCreate">The data for the new subscription</param>
        /// <returns>The created subscription</returns>
        public Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto subscriptionCreate, Guid userId);

        /// <summary>
        /// Update an existing subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to update</param>
        /// <param name="subscriptionUpdate">The data for the subscription to update</param>
        /// <param name="userId">The ID of the current user</param>
        /// <returns>The updated subscription</returns>
        public Task<SubscriptionDto> UpdateSubscriptionAsync(int subscriptionId, SubscriptionUpdateDto subscriptionUpdate, Guid userId);

        /// <summary>
        /// Get all the subscriptions for a specific user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="pageNumber">Index of the pagination</param>
        /// <returns>A paginated list of SubscriptionsResponseDto created by the user</returns>
        public Task<PaginatedSubscriptionsResponse> GetSubscryptionsByUserAsync(Guid userId, int pageNumber);

        /// <summary>
        /// Get All the subscriptions
        /// </summary>
        /// <param name="pageNumber">Index of the pagination</param>
        /// <returns>A list of subscriptionDto</returns>
        public Task<List<SubscriptionDto>> GetAllSubscryptionsAsync(int pageNumber);

        /// <summary>
        /// Get the subscription corresponding to the ID
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription</param>
        /// <param name="userId">ID of the current user</param>
        /// <returns>The subscription dto if existing, an 404 exception otherwise</returns>
        public Task<SubscriptionDto> GetSubscriptionByIdAsync(int subscriptionId, Guid userId);

        /// <summary>
        /// Delete a subscription
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to delete</param>
        /// <param name="userId">ID of the current user</param>
        public Task DeleteSubscriptionAsync(int subscriptionId, Guid userId);
    }
}
