using SubManager.Application.DTO.Subscription;

namespace SubManager.Application.DTO.Pagination
{
    public class PaginatedSubscriptionsResponse
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public List<SubscriptionDto> Items { get; set; } = new List<SubscriptionDto>();

        public decimal TotalCostMonth { get; set; }
        public decimal TotalCostYear { get; set; }
    }
}
