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