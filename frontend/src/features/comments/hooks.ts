import { useState } from "react";
import { apiClient } from "../../api/client";
import type { InfiniteScrollStatus } from "../pagination/inifinityScroll";
import type { PaginationResponse } from "../pagination/types";
import type { Comment } from "./types";

type ThreadOrder = "newestFirst" | "chronological"

function useCommentThread(endpoint: string, order: ThreadOrder) {
    const [status, setStatus] = useState<InfiniteScrollStatus>("idle");
    const [cursor, setCursor] = useState<string | null>(null);
    const [items, setItems] = useState<Comment[]>([]);

    const loadMore = async () => { 
        setStatus("loading");
        try {
            const response = await apiClient<PaginationResponse<Comment>>(
                cursor ? `${endpoint}?cursor=${cursor}` : endpoint
            );
            if (order === "chronological") {
                setItems(prev => {
                    const byId = new Map(prev.map(reply => [reply.id, reply]));

                    for (const reply of response.items) {
                        byId.set(reply.id, reply);
                    }

                    return sortItemByDate([...byId.values()]);
                });
            } else {
                setItems(prev => [...prev, ...response.items]);
            }
            setCursor(response.nextCursor);
            setStatus(response.hasMore ? "idle" : "finished");
        } catch {
            setStatus("error");
        }
    };

    const addItem = (item: Comment) => { 
        if (order === "chronological") {
            setItems(prev => sortItemByDate([...prev, item]));
        } else {
            setItems(prev => [item, ...prev]);
        }
    };

    const updateItem = (id: number, updater: (item: Comment) => Comment) => {
        setItems(prev => prev.map(item => item.id === id ? updater(item) : item));
    };

    const sortItemByDate = (replies: Comment[]): Comment[] => {
        return [...replies].sort((a, b) => {
            const dateDiff =
                new Date(a.createdAt).getTime() -
                new Date(b.createdAt).getTime();

            if (dateDiff !== 0) return dateDiff;

            return a.id - b.id;
        });
    }

    return { items, status, loadMore, addItem, updateItem };
}

export function useComments(postId: number) {
    const { items, status, loadMore, addItem, updateItem } = useCommentThread(
        `posts/${postId}/comments/`, "newestFirst"
    );
    return { comments: items, status, loadMore, addComment: addItem, updateComment: updateItem };
}

export function useReplies(postId: number, commentId: number) {
    const { items, status, loadMore, addItem, updateItem } = useCommentThread(
        `posts/${postId}/comments/${commentId}/replies`, "chronological"
    );
    return { replies: items, status, loadMore, addReply: addItem, updateReply: updateItem };
}