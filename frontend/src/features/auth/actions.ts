import type { ActionFunctionArgs } from "react-router";
import { apiClient, ApiError } from "../../api/client";
import type { LoginRequest, RegisterCompletionRequest, RegisterRequest, User } from "./types";
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
        await apiClient('auth/register', {
            method: "POST",
            body: JSON.stringify(registerRequest)
        });

        return {
            success: true
        }
    } catch (err) {
        if (err instanceof ApiError && (err.status == 409 || err.status === 400)) {
            return {
                success: false,
                error: err.message,
                validationErrors: err.validationErrors
            };
        }
        throw err;
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
        if (err instanceof ApiError && (err.status === 401 || err.status === 400)) {
            return {
                error: err.message
            };
        }
        throw err;
    }
}

export async function logoutAction() {
    await logout();
}

export async function completeRegisterAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    const registerRequest: RegisterCompletionRequest = {
        userTag: getFormString(formData, "userTag"),
        token: getFormString(formData, "token"),
    };

    try {
        const user = await apiClient<User>('auth/register/complete', {
            method: "POST",
            body: JSON.stringify(registerRequest)
        });

        authStore.setUser(user);
    } catch (err) {
        if (err instanceof ApiError && (err.status == 409 || err.status === 400)) {
            return {
                error: err.message,
                validationErrors: err.validationErrors
            };
        }
        throw err;
    }
}
