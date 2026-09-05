import type { ActionFunctionArgs } from "react-router";
import { apiClient, ApiError } from "../../api/client";
import type { LoginRequest, RegisterRequest, User } from "./types";
import { getFormString } from "../../utils/formDataUtils";
import { authStore } from "./store";
import { logout } from "./api";

export async function registerAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    const registerRequest: RegisterRequest = {
        userName: getFormString(formData, "userName"),
        userTag: getFormString(formData, "userTag"),
        email: getFormString(formData, "email"),
        password: getFormString(formData, "password"),
    };

    try {
        const user = await apiClient<User>('auth/register', {
            method: "POST",
            body: JSON.stringify(registerRequest)
        });

        authStore.setUser(user);
    } catch (err) {
        if (err instanceof ApiError && err.status == 409) {
            return {
                error: err.message
            };
        }
    }
}

export async function loginAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    const loginRequest: LoginRequest = {
        login: getFormString(formData, "login"),
        password: getFormString(formData, "password"),
    };

    try {
        const user = await apiClient<User>('auth/login', {
            method: "POST",
            body: JSON.stringify(loginRequest)
        });

        authStore.setUser(user);
    } catch (err) {
        if (err instanceof ApiError && err.status == 401) {
            return {
                error: err.message
            };
        }
    }
}

export async function logoutAction() {
    await logout();
}
