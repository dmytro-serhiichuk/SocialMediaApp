import { apiClient, ApiError } from "../../api/client";
import type { ConfirmationRequest } from "../confirmation/types";
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

export async function confirmRegister(token: string) {
    var req: ConfirmationRequest = {
        token: token,
    };

    const user = await apiClient<User>("auth/register/confirm", { 
        method: "POST",
        body: JSON.stringify(req)
    });

    authStore.setUser(user);
}

export async function logout() {
    await apiClient('auth/logout', { method: "POST" });
    authStore.clearUser();
}
