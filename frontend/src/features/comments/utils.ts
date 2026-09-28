import type { Comment } from "./types";

export function toggleLike(comment: Comment): Comment {
    return {
        ...comment,
        isLikedByMe: !comment.isLikedByMe,
        likesCount: comment.isLikedByMe ? comment.likesCount - 1 : comment.likesCount + 1
    };
}