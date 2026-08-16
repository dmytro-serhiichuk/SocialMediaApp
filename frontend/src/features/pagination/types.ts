export interface PaginationResponse<T> {
    items: T[],
    hasMore: boolean,
    nextCursor: string | null
}