import { apiClient, ApiError } from "../../api/client";
import { authStore } from "./store";
import type { User } from "./types";

export async function getCurrentUser(): Promise<User | null> {
    try {
        return await apiClient<User>('auth/me/');
    } catch (err) {
        if (err instanceof ApiError && err.status == 401) {
            return null;
        }
        throw err;
    }
}

export async function logout() {
    await apiClient('auth/logout', { method: "POST" });
    authStore.clearUser();
}
