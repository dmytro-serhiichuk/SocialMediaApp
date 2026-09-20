import { useState } from "react";
import { apiClient } from "../../api/client";
import type { InfiniteScrollStatus } from "../pagination/inifinityScroll";
import type { PaginationResponse } from "../pagination/types";

export function useComments(id: number) {
    const [status, setStatus] = useState<InfiniteScrollStatus>("idle");
    const [cursor, setCursor] = useState<string | null>(null);
    const [comments, setComments] = useState<Comment[]>([]);
    const endpoint = `posts/${id}/comments/`;

    const loadMore = async () => {
        setStatus("loading");
        try {
            const response = await apiClient<PaginationResponse<Comment>>(
                cursor ? `${endpoint}?cursor=${cursor}` : endpoint
            );
            setComments(prev => [...prev, ...response.items]);
            setCursor(response.nextCursor);
            setStatus(response.hasMore ? "idle" : "finished");
        } catch {
            setStatus("error");
        }
    };

    const addComment = (comment: Comment) => {
        setComments(prev => [comment, ...prev]);
    };

    return { comments, status, loadMore, addComment };
}