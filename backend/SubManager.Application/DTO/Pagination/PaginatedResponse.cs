namespace SubManager.Application.DTO.Pagination
{
    public class PaginatedResponse<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public T? Items { get; set; } 
    }
}
