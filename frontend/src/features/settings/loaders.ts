import { redirect } from "react-router";
import { apiClient } from "../../api/client";
import { authStore } from "../auth/store";
import type { SettingsDataResponse } from "./types";
import { ROUTES } from "../../constants/routes";
import { logout } from "../auth/api";

export async function settingsLoader() {
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

    return data;
}