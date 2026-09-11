export interface SettingsDataResponse {
    id: number,
    userName: string,
    userTag: string,
    email: string,
    sessions: SessionInfo[],
    hasPassword: boolean,
}

export interface SessionInfo {
    id: number,
    firstLogin: string,
    lastLogin: string,
    deviceInfo: string | null,
    ip: string | null,
    isCurrentSession: boolean
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