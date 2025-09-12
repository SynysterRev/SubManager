using SubManager.Application.Commons;
using SubManager.Application.DTO.Pagination;

namespace SubManager.Application.Extensions
{
    public static class PaginationExtensions
    {
        public static PaginatedResponse<TResult> ToResponse<TSource, TResult>(
            this PaginatedList<TSource> list,
            TResult items)
        {
            return new PaginatedResponse<TResult>
            {
                PageIndex = list.PageIndex,
                TotalPages = list.TotalPages,
                TotalCount = list.TotalCount,
                HasNextPage = list.HasNextPage,
                HasPreviousPage = list.HasPreviousPage,
                Items = items,
            };
        }
    }
}
