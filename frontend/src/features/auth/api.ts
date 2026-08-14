import { apiClient, ApiError } from "../../api/client";
import type { User } from "./types";

export async function getUser(): Promise<User | null> {
    try {
        return await apiClient<User>('auth/me/');
    } catch (err) {
        if (err instanceof ApiError && err.status == 401) {
            return null;
        }
        throw err;
    }
}
