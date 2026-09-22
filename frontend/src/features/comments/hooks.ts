import { useState } from "react";
import { apiClient } from "../../api/client";
import type { InfiniteScrollStatus } from "../pagination/inifinityScroll";
import type { PaginationResponse } from "../pagination/types";
import type { Comment } from "./types";

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

export function useReplies(postId: number, commentId: number) {
    const [status, setStatus] = useState<InfiniteScrollStatus>("idle");
    const [cursor, setCursor] = useState<string | null>(null);
    const [replies, setReplies] = useState<Comment[]>([]);
    const endpoint = `posts/${postId}/comments/${commentId}/replies`;

    const loadMore = async () => {
        setStatus("loading");
        try {
            const response = await apiClient<PaginationResponse<Comment>>(
                cursor ? `${endpoint}?cursor=${cursor}` : endpoint
            );
            setReplies(prev => {
                const byId = new Map(prev.map(reply => [reply.id, reply]));

                for (const reply of response.items) {
                    byId.set(reply.id, reply);
                }

                return sortReplies([...byId.values()]);
            });
            setCursor(response.nextCursor);
            setStatus(response.hasMore ? "idle" : "finished");
        } catch {
            setStatus("error");
        }
    };

    const addReply = (reply: Comment) => {
        setReplies(prev => sortReplies([...prev, reply]));
    };

    const sortReplies = (replies: Comment[]): Comment[] => {
        return [...replies].sort((a, b) => {
            const dateDiff =
                new Date(a.createdAt).getTime() -
                new Date(b.createdAt).getTime();

            if (dateDiff !== 0) return dateDiff;

            return a.id - b.id;
        });
    }

    return { replies, status, loadMore, addReply };
}