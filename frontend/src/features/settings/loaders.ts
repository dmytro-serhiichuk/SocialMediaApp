import { apiClient } from "../../api/client";
import { authStore } from "../auth/store";
import type { SettingsDataResponse } from "./types";

export async function settingsLoader() {
    const data = await apiClient<SettingsDataResponse>(`settings`);

    let currentUser = authStore.getUser(); 
    
    if (currentUser?.id === data.id) {
        authStore.updateUser({
            userName: data.userName,
            userTag: data.userTag
        });
    }

    return data;
}