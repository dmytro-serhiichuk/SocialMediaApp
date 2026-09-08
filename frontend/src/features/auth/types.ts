export interface User {
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

export interface RequestPasswordResetRequest {
    email: string
}

export interface ResetPasswordRequest {
    token: string,
    newPassword: string
}