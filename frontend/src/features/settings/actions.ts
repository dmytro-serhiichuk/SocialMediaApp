import type { ActionFunctionArgs } from "react-router";
import { SETTINGS_INTENTS, type SettingsProfileUpdateRequest } from "./types";
import { getFormString } from "../../utils/formDataUtils";
import { apiClient, ApiError } from "../../api/client";

export async function settingsAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();
    const intent = formData.get("intent");

    switch (intent) {
        case SETTINGS_INTENTS.PROFILE_CHANGE:
            return profileAction(formData)
        default:
            throw new Response("Invalid settingsintent", { status: 400 });
    }
}

async function profileAction(formData: FormData) {
    const req: SettingsProfileUpdateRequest = {
        userName: getFormString(formData, "userName"),
        userTag: getFormString(formData, "userTag"),
    };

    try {
        await apiClient('settings/profile', {
            method: "PATCH",
            body: JSON.stringify(req)
        });
    } catch (err) {
        if (err instanceof ApiError && err.status == 409) {
            return {
                intent: SETTINGS_INTENTS.PROFILE_CHANGE,
                error: err.message
            };
        }
    } 
}