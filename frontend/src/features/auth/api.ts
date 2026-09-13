import { apiClient, ApiError } from "../../api/client";
import type { ConfirmationRequest } from "../confirmation/types";
import { authStore } from "./store";
import type { AuthUser } from "./types";

export async function getCurrentUser(): Promise<AuthUser | null> {
    try {
        return await apiClient<AuthUser>('auth/me/');
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

    const user = await apiClient<AuthUser>("auth/register/confirm", { 
        method: "POST",
        body: JSON.stringify(req)
    });

    authStore.setUser(user);
}

export async function logout() {
    await apiClient('auth/logout', { method: "POST" });
    authStore.clearUser();
}

export async function secureAccount(token: string) {
    var req: ConfirmationRequest = {
        token: token,
    };

    await apiClient<AuthUser>("auth/acccount-recovery/secure", { 
        method: "POST",
        body: JSON.stringify(req)
    });
}
