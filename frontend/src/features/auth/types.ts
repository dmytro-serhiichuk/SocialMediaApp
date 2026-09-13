export interface AuthUser {
    id: number,
    userName: string,
    userTag: string
}

export interface LoginRequest {
    login: string,
    password: string
}

export interface RegisterRequest {
    userName: string,
    userTag: string,
    email: string,
    password: string
}

export interface RegisterCompletionRequest {
    userTag: string,
    token: string
}

export type RequestPasswordResetType = "Reset" | "Create";

export interface RequestPasswordResetRequest {
    email: string,
    requestType: RequestPasswordResetType
}

export interface ResetPasswordRequest {
    token: string,
    newPassword: string
}