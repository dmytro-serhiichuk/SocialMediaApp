import { redirect } from "react-router";
import { apiClient, ApiError } from "../../api/client";
import { authStore } from "../auth/store";
import type { SettingsDataResponse } from "./types";
import { ROUTES } from "../../constants/routes";
import { logout } from "../auth/api";
import { confirmEmailChange } from "./api";
import { getExternalLoginError } from "../../utils/queryUtils";

type SettingsLoaderData = {
    data: SettingsDataResponse,
    providerConnectionError: string | null
}

export async function settingsLoader(): Promise<SettingsLoaderData> {
    const data = await apiClient<SettingsDataResponse>(`settings`);

    let currentUser = authStore.getUser(); 
    
    if (currentUser?.id === data.id) {
        authStore.updateUser({
            userName: data.userName,
            userTag: data.userTag
        });
    }

    if (!data.sessions.some(x => x.isCurrentSession)) {
        await logout();
        throw redirect(ROUTES.LOGOUT);
    }

    var providerConnectionError = getExternalLoginError();

    return { data, providerConnectionError };
}

export async function emailChangeConfirmLoader(): Promise<boolean> {
    const params = new URLSearchParams(window.location.hash.slice(1));
    const token = params.get("token");
    window.history.replaceState(null, "", window.location.pathname + window.location.search);

    if (!token) return false;

    const expectedErrors = [400, 409];

    try {
        await confirmEmailChange(token);
        return true;
    } catch (error) {
        if (error instanceof ApiError && expectedErrors.includes(error.status)) {
            return false;
        }
        throw error;
    }
}