import type { UserResponse } from "../user/types";

export interface Comment {
    id: number,
    user: UserResponse,
    content: string,
    createdAt: string,
    updatedAt: string | null,
    replyToUserTag: string | null,
    repliesCount: number
}

export interface CommentRequest {
    content: string
}