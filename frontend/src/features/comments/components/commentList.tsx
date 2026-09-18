import { useState } from "react";
import { InfinityScroll, type InfiniteScrollStatus } from "../../pagination/inifinityScroll";
import type { Comment } from "../types";
import { apiClient } from "../../../api/client";
import type { PaginationResponse } from "../../pagination/types";
import CommentComponent from "./comment";

type CommentListProps = {
    id: number
}

export default function CommentList({ id }: CommentListProps) {
    const [state, setStatus]  = useState<InfiniteScrollStatus>("idle");
    const [cursor, setCursor] = useState<string | null>(null);
    const [comments, setComments]   = useState<Comment[]>([]);

    const endpoint = `posts/${id}/comments/`;

    const loadMore = async () => {
        setStatus("loading");

        try {
            const response = await apiClient<PaginationResponse<Comment>>(cursor ? endpoint + `?cursor=${cursor}` : endpoint);

            setComments(prev => [...prev, ...response.items]);
            setCursor(response.nextCursor);
            setStatus(response.hasMore ? "idle" : "finished");
        } catch {
            setStatus("error");
        }
    }

    return (
        <div className="comments-list">
            <h3>Comments:</h3>
                    
            {/* <hr /> */}

            {comments.map(comment => (
                <CommentComponent comment={comment} key={comment.id} />
            ))}

            <InfinityScroll 
                status={state}
                onLoadMore={loadMore} />
        </div>
    );
}