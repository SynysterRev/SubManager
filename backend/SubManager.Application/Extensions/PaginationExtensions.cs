using SubManager.Application.Commons;
using SubManager.Application.DTO.Pagination;
using SubManager.Application.DTO.Subscription;
using SubManager.Domain.Entities;

namespace SubManager.Application.Extensions
{
    public static class PaginationExtensions
    {
        public static PaginatedSubscriptionsResponse ToResponse(
            this PaginatedList<Subscription> list,
            List<SubscriptionDto> subscriptionDtos,
            float totalCostMonth)
        {
            return new PaginatedSubscriptionsResponse
            {
                PageIndex = list.PageIndex,
                TotalPages = list.TotalPages,
                TotalCount = list.TotalCount,
                HasNextPage = list.HasNextPage,
                HasPreviousPage = list.HasPreviousPage,
                Items = subscriptionDtos,
                TotalCostMonth = totalCostMonth,
                TotalCostYear = totalCostMonth * 12
            };
        }
    }
}
