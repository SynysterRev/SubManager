using JuniorOnly.Application.Commons;
using Microsoft.Extensions.Options;
using SubManager.Application.Commons;
using SubManager.Application.DTO.Subscription;
using SubManager.Application.Exceptions;
using SubManager.Application.Extensions;
using SubManager.Application.Interfaces;
using SubManager.Domain.Entities;
using SubManager.Domain.Repositories;
using System.ComponentModel.DataAnnotations;

namespace SubManager.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly PaginationOptions _paginationOptions;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IOptions<PaginationOptions> paginationOptions)
        {
            _subscriptionRepository = subscriptionRepository;
            _paginationOptions = paginationOptions.Value;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto subscriptionCreate, Guid userId)
        {
            Validator.ValidateObject(subscriptionCreate, new ValidationContext(subscriptionCreate), validateAllProperties: true);

            var newSubscription = subscriptionCreate.ToEntity();
            newSubscription.UserId = userId;
            var subscription = await _subscriptionRepository.AddSubscriptionAsync(newSubscription);
            return subscription.ToDto();
        }

        public async Task DeleteSubscriptionAsync(int subscriptionId, Guid userId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);

            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with id {subscriptionId} not found");
            }

            if (subscription.UserId != userId)
            {
                throw new UnauthorizedAccessException("You cannot update this subscription");
            }

            await _subscriptionRepository.DeleteSubscriptionAsync(subscription);
        }

        public async Task<List<SubscriptionDto>> GetAllSubscryptionsAsync(int pageNumber)
        {
            var subscriptions = await PaginatedList<Subscription>.CreateAsync(
                _subscriptionRepository.GetAllSubscriptions(),
                pageNumber,
                _paginationOptions.DefaultPageSize);
            return subscriptions.Select(s => s.ToDto()).ToList();
        }

        public async Task<SubscriptionDto> GetSubscriptionByIdAsync(int subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with id {subscriptionId} not found");
            }
            return subscription.ToDto();
        }

        public async Task<List<SubscriptionDto>> GetSubscryptionsByUserAsync(Guid userId, int pageNumber)
        {
            var subscriptions = await PaginatedList<Subscription>.CreateAsync(
                _subscriptionRepository.GetAllSubscriptionsByUser(userId),
                pageNumber,
                _paginationOptions.DefaultPageSize);
            return subscriptions.Select(s => s.ToDto()).ToList();
        }

        public async Task<SubscriptionDto> UpdateSubscriptionAsync(int subscriptionId, SubscriptionUpdateDto subscriptionUpdate, Guid userId)
        {
            Validator.ValidateObject(subscriptionUpdate, new ValidationContext(subscriptionUpdate), validateAllProperties: true);


            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with id {subscriptionId} not found");
            }

            if (subscription.UserId != userId)
            {
                throw new UnauthorizedAccessException("You cannot update this subscription");
            }

            subscription.UpdateFrom(subscriptionUpdate);
            await _subscriptionRepository.SaveChangesAsync();
            return subscription.ToDto();
        }
    }
}
