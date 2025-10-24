using JuniorOnly.Application.Commons;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SubManager.Application.Commons;
using SubManager.Application.DTO.Pagination;
using SubManager.Application.DTO.Subscription;
using SubManager.Application.Exceptions;
using SubManager.Application.Extensions;
using SubManager.Application.Interfaces;
using SubManager.Domain.Entities;
using SubManager.Domain.IdentityEntities;
using SubManager.Domain.Repositories;
using System.ComponentModel.DataAnnotations;

namespace SubManager.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PaginationOptions _paginationOptions;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IOptions<PaginationOptions> paginationOptions,
            UserManager<ApplicationUser> userManager,
            ICategoryRepository categoryRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _paginationOptions = paginationOptions.Value;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto subscriptionCreate, Guid userId)
        {
            Validator.ValidateObject(subscriptionCreate, new ValidationContext(subscriptionCreate), validateAllProperties: true);

            Category? category = null;

            if (subscriptionCreate.CategoryId.HasValue)
            {
                category = await _categoryRepository.GetByIdAsync(subscriptionCreate.CategoryId.Value);
                if (category == null)
                {
                    throw new NotFoundException($"Category with ID {subscriptionCreate.CategoryId.Value} not found");
                }
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException($"User with ID {userId} not found");
            }

            var newSubscription = subscriptionCreate.ToEntity();
            newSubscription.UserId = userId;
            newSubscription.Category = category;
            var subscription = await _subscriptionRepository.AddSubscriptionAsync(newSubscription);
            return subscription.ToDto();
        }

        public async Task DeleteSubscriptionAsync(int subscriptionId, Guid userId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId, userId);

            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with id {subscriptionId} not found");
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

        public async Task<SubscriptionDto> GetSubscriptionByIdAsync(int subscriptionId, Guid userId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId, userId);
            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with id {subscriptionId} not found");
            }

            return subscription.ToDto();
        }

        public async Task<PaginatedSubscriptionsResponse> GetSubscryptionsByUserAsync(Guid userId, int pageNumber)
        {
            var subQuery = _subscriptionRepository.GetAllSubscriptionsByUser(userId);

            var paginatedResult = await PaginatedList<Subscription>.CreateAsync(
                subQuery,
                pageNumber,
                _paginationOptions.DefaultPageSize);

            decimal totalCost = await subQuery.Where(s => s.IsActive)
                .SumAsync(s => s.Price);

            var subscriptionDtos = paginatedResult.Select(s => s.ToDto()).ToList();

            return paginatedResult.ToResponse(subscriptionDtos, totalCost);
        }

        public async Task<SubscriptionDto> UpdateSubscriptionAsync(int subscriptionId, SubscriptionUpdateDto subscriptionUpdate, Guid userId)
        {
            Validator.ValidateObject(subscriptionUpdate, new ValidationContext(subscriptionUpdate), validateAllProperties: true);


            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId, userId);
            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with id {subscriptionId} not found");
            }

            Category? category = null;

            if (subscriptionUpdate.CategoryId.HasValue)
            {
                category = await _categoryRepository.GetByIdAsync(subscriptionUpdate.CategoryId.Value);
                if (category == null)
                {
                    throw new NotFoundException($"Category with ID {subscriptionUpdate.CategoryId.Value} not found");
                }
            }

            subscription.UpdateFrom(subscriptionUpdate);
            await _subscriptionRepository.SaveChangesAsync();
            return subscription.ToDto();
        }
    }
}
