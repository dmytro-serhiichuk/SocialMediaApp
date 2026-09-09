import { ApiError } from "../../api/client";
import { getCurrentUser, secureAccount } from "./api";
import { authStore } from "./store";

export async function authLoader() {
    const user = await getCurrentUser();
    authStore.setUser(user);
}

type AccountRecoveryLoaderData = {
    token?: string,
    tokenIsNotValid?: boolean,
    recoveryNotAvailable?: boolean
}

export async function accountRecoveryLoader(): Promise<AccountRecoveryLoaderData> {
    const params = new URLSearchParams(window.location.hash.slice(1));
    const token = params.get("token");
    window.history.replaceState(null, "", window.location.pathname + window.location.search);

    if (!token) return { };

    try {
        await secureAccount(token);
        return { token: token }
    } catch (error) {
        if (error instanceof ApiError && [400, 409].includes(error.status)) {
            return {
                token: token,
                tokenIsNotValid: error.status === 400,
                recoveryNotAvailable: error.status === 409
            }
        }
        throw error;
    }
}