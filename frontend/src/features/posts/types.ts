import type { User } from "../auth/types";
import type { Image } from "../image/types";
import type { Tag } from "../tag/types";

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
    images: Image[]
}