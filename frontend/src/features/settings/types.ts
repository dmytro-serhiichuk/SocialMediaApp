import { API_URL } from "../../constants"

export interface SettingsDataResponse {
    id: number,
    userName: string,
    userTag: string,
    email: string,
    sessions: SessionInfo[],
    hasPassword: boolean,
    connectedLoginProviders: string[] 
}

export type SessionState = 
    | "Active"
    | "LoggedOut"
    | "ReuseDetected"
    | "Terminated"

export interface SessionInfo {
    id: number,
    firstLogin: string,
    lastLogin: string,
    deviceInfo: string | null,
    ip: string | null,
    isCurrentSession: boolean,
    state: SessionState
}

export interface SettingsProfileUpdateRequest {
    userName: string,
    userTag: string
}

export interface EmailChangeRequest {
    newEmail: string
}

export interface PasswrodChangeRequest {
    oldPassword: string
    newPassword: string
}

export const SETTINGS_INTENTS = {
    PROFILE_CHANGE:  "profile-change",
    EMAIL_CHANGE:    "email-change",
    PASSWORD_CHANGE: "password-change",
    PASSWORD_CREATE: "password-create"
} as const;

export const EXTERNAL_LOGIN_PROVIDERS = [
    {
        name: "Google",
        signInEndpoint: `${API_URL}auth/google`,
        connectEndpoint: `${API_URL}auth/google/connect`
    }
] as const;

export type ExternalLoginProvider = typeof EXTERNAL_LOGIN_PROVIDERS[number];