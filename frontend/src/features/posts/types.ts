import type { Image } from "../image/types";
import type { Tag } from "../tag/types";
import type { UserResponse } from "../user/types";

export interface Post {
    id: number,
    user: UserResponse,
    content: string,
    createdAt: string,
    updatedAt: string | null,
    likesCount: number,
    isLikedByMe: boolean,
    tags: Tag[],
    isSavedByMe: boolean,
    images: Image[]
}

export interface PostEditInfo {
    id: number,
    content: string,
    tags: string[],
    images: Image[]
}