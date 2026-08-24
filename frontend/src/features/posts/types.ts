import type { User } from "../auth/types";
import type { Tag } from "../tag/types";

export interface PostForm {
    content: string,
    tags: string[]
}

export interface Post {
    id: number,
    user: User,
    content: string,
    createdAt: string,
    updatedAt: string | null,
    likesCount: number,
    isLikedByMe: boolean,
    tags: Tag[],
    isSavedByMe: boolean,
}