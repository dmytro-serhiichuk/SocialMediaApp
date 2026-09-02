export interface SettingsDataResponse {
    id: number,
    userName: string,
    userTag: string,
    email: string,
}

export interface SettingsProfileUpdateRequest {
    userName: string,
    userTag: string
}

export interface EmailChangeRequest {
    newEmail: string
}

export interface EmailChangeConfirmation {
    token: string
}

export const SETTINGS_INTENTS = {
    PROFILE_CHANGE:  "profile-change",
    EMAIL_CHANGE:    "email-change",
    PASSWORD_CHANGE: "password-change"
} as const;