export interface User {
    userName: string,
    userTag: string,
    email: string
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