import { ApiError } from "../../api/client";
import { confirmRegister, getCurrentUser, secureAccount } from "./api";
import { authStore } from "./store";

export async function authLoader() {
    const user = await getCurrentUser();
    authStore.setUser(user);
}

type RegisterConfirmationLoaderData = 
    | { type: "success" }
    | { type: "invalid-token" }
    | { type: "email-conflict" }
    | { type: "tag-conflict"; token: string };

export async function registerConfirmationLoader(): Promise<RegisterConfirmationLoaderData> {
    const params = new URLSearchParams(window.location.hash.slice(1));
    const token = params.get("token");
    window.history.replaceState(null, "", window.location.pathname + window.location.search);

    if (!token) return { type: "invalid-token" };

    const expectedStatuses = [400, 409];

    try {
        await confirmRegister(token);

        return { type: "success" };
    } catch (error) {
        if (error instanceof ApiError && expectedStatuses.includes(error.status)) {
            if (error.validationErrors?.email) {
                return { type: "email-conflict" };
            } else if (error.validationErrors?.tag) {
                return { type: "tag-conflict", token: token };
            } else {
                return { type: "invalid-token" };
            }
        }
        throw error;
    }
}

export function completePasswordResetLoader(): string | null {
    const params = new URLSearchParams(window.location.hash.slice(1));
    const token = params.get("token");
    window.history.replaceState(null, "", window.location.pathname + window.location.search);

    return token;
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

export function oauthLoader() {
    const url = new URL(window.location.href);
    const error = url.searchParams.get("error");
    const provider = url.searchParams.get("provider");

    window.history.replaceState(null, "", url.pathname);

    if (error === "oauth_failed") {
        return "Authentication failed. Please try again";
    } else if (error === "email_taken") {
        return `An account with this email already exists. Sign in to your existing account and connect ${provider} from Settings`
    } else if (error === "unknown") {
        throw new Error();
    }

    return null
}