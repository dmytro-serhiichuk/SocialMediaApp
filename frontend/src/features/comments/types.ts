import type { UserResponse } from "../user/types";

export interface Comment {
    id: number,
    user: UserResponse,
    content: string,
    createdAt: string,
    updatedAt: string | null,
    replyToUserTag: string | null,
    repliesCount: number,
    likesCount: number,
    isLikedByMe: boolean
}

export interface CommentRequest {
    content: string
}