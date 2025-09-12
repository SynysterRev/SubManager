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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PaginationOptions _paginationOptions;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IOptions<PaginationOptions> paginationOptions, UserManager<ApplicationUser> userManager)
        {
            _subscriptionRepository = subscriptionRepository;
            _paginationOptions = paginationOptions.Value;
            _userManager = userManager;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto subscriptionCreate, Guid userId)
        {
            Validator.ValidateObject(subscriptionCreate, new ValidationContext(subscriptionCreate), validateAllProperties: true);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException($"User with ID {userId} not found");
            }

            var newSubscription = subscriptionCreate.ToEntity();
            newSubscription.UserId = userId;
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

        public async Task<PaginatedResponse<SubscriptionsResponseDto>> GetSubscryptionsByUserAsync(Guid userId, int pageNumber)
        {
            var subQuery = _subscriptionRepository.GetAllSubscriptionsByUser(userId);
            var paginatedTask = PaginatedList<Subscription>.CreateAsync(
                _subscriptionRepository.GetAllSubscriptionsByUser(userId),
                pageNumber,
                _paginationOptions.DefaultPageSize);
            var totalCostTask = subQuery.Where(s => s.IsActive)
                .SumAsync(s => s.Price);

            await Task.WhenAll(paginatedTask, totalCostTask);

            var subscriptionDtos = paginatedTask.Result.Select(s => s.ToDto()).ToList();

            var response = new SubscriptionsResponseDto
            {
                Subscriptions = subscriptionDtos,
                TotalCost = totalCostTask.Result,
            };

            return paginatedTask.Result.ToResponse(response);
        }

        public async Task<SubscriptionDto> UpdateSubscriptionAsync(int subscriptionId, SubscriptionUpdateDto subscriptionUpdate, Guid userId)
        {
            Validator.ValidateObject(subscriptionUpdate, new ValidationContext(subscriptionUpdate), validateAllProperties: true);


            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId, userId);
            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with id {subscriptionId} not found");
            }

            subscription.UpdateFrom(subscriptionUpdate);
            await _subscriptionRepository.SaveChangesAsync();
            return subscription.ToDto();
        }
    }
}
