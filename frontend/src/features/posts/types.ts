import type { User } from "../auth/types";

export interface PostForm {
    content: string
}

export interface Post {
    id: number,
    user: User,
    content: string,
    createdAt: string,
    updatedAt: string | null
}