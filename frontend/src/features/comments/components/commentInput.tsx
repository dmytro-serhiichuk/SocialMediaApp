import { useState } from "react";
import { apiClient, ApiError } from "../../../api/client";
import type { CommentRequest, Comment } from "../types";

type CommentInputProps = {
    postId: number,
    onCommentCreated: (comment: Comment) => void,
    repliedCommentId?: number
}

export default function CommentInput({ postId, onCommentCreated, repliedCommentId }: CommentInputProps) {
    const [content, setContent] = useState("");
    const [error, setError] = useState<string>();

    const handleClick = async () => {
        setError(undefined);

        const body: CommentRequest = {
            content: content
        }

        try {
            const endpoint = repliedCommentId ? `posts/${postId}/comments/${repliedCommentId}/reply`
                                       : `posts/${postId}/comment`;

            const res = await apiClient<Comment>(endpoint, {
                method: "post",
                body: JSON.stringify(body)
            });

            setContent("");

            onCommentCreated(res);
        } catch (error) {
            if (error instanceof ApiError && error.status === 400) {
                setError(error.message);
            } else {
                throw error;
            }
        }
        
    }
    
    return (
        <div>
            {error && <p>{error}</p>}

            <textarea
                name="comment"
                placeholder="Enter your comment..." 
                value={content} 
                onChange={e => setContent(e.target.value)} />

            <button 
                disabled={content.length === 0}
                onClick={handleClick}>
                {repliedCommentId ? "Reply" : "Comment"}
            </button>
        </div>
    );
}