import { useState } from "react";
import { apiClient, ApiError } from "../../../api/client";
import type { CommentPostRequest, Comment } from "../types";

type CommentInputProps = {
    id: number,
    onCommentCreated: (comment: Comment) => void
}

export default function CommentInput({ id, onCommentCreated }: CommentInputProps) {
    const [content, setContent] = useState("");
    const [error, setError] = useState<string>();

    const handleClick = async () => {
        setError(undefined);

        const body: CommentPostRequest = {
            content: content
        }

        try {
            const res = await apiClient<Comment>(`posts/${id}/comment`, {
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
        <>
            {error && <p>{error}</p>}

            <textarea
                placeholder="Enter your comment..." 
                value={content} 
                onChange={e => setContent(e.target.value)} />

            <button 
                disabled={content.length === 0}
                onClick={handleClick}>
                Comment
            </button>
        </>
    );
}