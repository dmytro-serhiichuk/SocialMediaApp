export interface Tag {
    id: number,
    name: string
};

export interface TagSearchResponse {
    name: string,
    useCount: number
};

export interface TagSearchRequest {
    query: string
};