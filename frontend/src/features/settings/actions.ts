import type { ActionFunctionArgs } from "react-router";
import { SETTINGS_INTENTS, type EmailChangeRequest, type PasswrodChangeRequest, type SettingsProfileUpdateRequest } from "./types";
import { getFormString } from "../../utils/formDataUtils";
import { apiClient, ApiError } from "../../api/client";
import { authStore } from "../auth/store";

export async function settingsAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();
    const intent = formData.get("intent");

    switch (intent) {
        case SETTINGS_INTENTS.PROFILE_CHANGE:
            return profileAction(formData);
        case SETTINGS_INTENTS.EMAIL_CHANGE:
            return emailAction(formData);
        case SETTINGS_INTENTS.PASSWORD_CHANGE:
            return passwordAction(formData);
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

        authStore.updateUser({
            userName: req.userName,
            userTag: req.userTag
        });
    } catch (err) {
        if (err instanceof ApiError && err.status === 409) {
            return {
                intent: SETTINGS_INTENTS.PROFILE_CHANGE,
                error: err.message
            };
        }
        throw err;
    } 
}

async function emailAction(formData: FormData) {
    const req: EmailChangeRequest = {
        newEmail: getFormString(formData, "email"),
    };

    try {
        await apiClient('settings/email-change', {
            method: "POST",
            body: JSON.stringify(req)
        });
    } catch (err) {
        if (err instanceof ApiError && err.status === 409) {
            return {
                intent: SETTINGS_INTENTS.EMAIL_CHANGE,
                error: err.message
            };
        }
        throw err;
    }
}

async function passwordAction(formData: FormData) {
    const req: PasswrodChangeRequest = {
        oldPassword: getFormString(formData, "old-password"),
        newPassword: getFormString(formData, "new-password"),
    };

    try {
        await apiClient('settings/password-change', {
            method: "POST",
            body: JSON.stringify(req)
        });
    } catch (err) {
        if (err instanceof ApiError && err.status === 400) {
            return {
                intent: SETTINGS_INTENTS.PASSWORD_CHANGE,
                error: err.message
            };
        }
        throw err;
    }
}