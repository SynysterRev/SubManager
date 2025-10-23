export interface PaginatedResponse {
    pageIndex: number;
    totalPages: number;
    totalCount: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
    items: [];
    totalCostMonth: number;
    totalCostYear: number;
}