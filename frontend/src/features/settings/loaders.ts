import { apiClient } from "../../api/client";
import type { SettingsDataResponse } from "./types";

export function settingsLoader() {
    return apiClient<SettingsDataResponse>(`settings`);
}