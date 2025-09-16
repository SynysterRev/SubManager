using SubManager.Application.DTO.Subscription;
using SubManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Application.Extensions
{
    public static class SubscriptionExtensions
    {
        public static SubscriptionDto ToDto(this Subscription subscription)
        {
            var today = DateTime.UtcNow;

            var nextPayment = new DateTime(today.Year, today.Month, subscription.PaymentDay);
            if (nextPayment < today)
                nextPayment = nextPayment.AddMonths(1);

            return new SubscriptionDto
            {
                Id = subscription.Id,
                Category = subscription.Category ?? "",
                CreatedAt = subscription.CreatedAt,
                IsActive = subscription.IsActive,
                Name = subscription.Name,
                Price = subscription.Price,
                PaymentDate = nextPayment,
                DaysBeforeNextPayment = (nextPayment - today).Days,
                YearCost = subscription.Price * 12,
                UserId = subscription.UserId,
            };
        }

        public static Subscription ToEntity(this SubscriptionCreateDto createDto)
        {
            return new Subscription
            {
                Name = createDto.Name,
                Category = createDto.Category,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Price = createDto.Price,
                PaymentDay = createDto.PaymentDay,
            };
        }

        public static void UpdateFrom(this Subscription subscription, SubscriptionUpdateDto updateDto)
        {
            if (updateDto.IsActive.HasValue)
            {
                subscription.IsActive = updateDto.IsActive.Value;
            }
            if (updateDto.PaymentDay.HasValue)
            {
                subscription.PaymentDay = updateDto.PaymentDay.Value;
            }
            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                subscription.Name = updateDto.Name;
            }
            if (!string.IsNullOrEmpty(updateDto.Category))
            {
                subscription.Category = updateDto.Category;
            }
            if (updateDto.Price.HasValue)
            {
                subscription.Price = updateDto.Price.Value;
            }
        }
    }
}
