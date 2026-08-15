import { redirect, type ActionFunctionArgs } from "react-router";
import { apiClient, ApiError } from "../../api/client";
import { ROUTES } from "../../constants/routes";
import type { LoginRequest, RegisterRequest } from "./types";
import { getFormString } from "../../utils/formDataUtils";

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

        return redirect(ROUTES.PROFILE);
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
        await apiClient('auth/login', {
            method: "POST",
            body: JSON.stringify(loginRequest)
        });

        return redirect(ROUTES.PROFILE);
    } catch (err) {
        if (err instanceof ApiError && err.status == 401) {
            return {
                error: err.message
            };
        }
    }
}

export async function logoutAction() {
    try {
        await apiClient('auth/logout', { method: "POST" });

        return redirect(ROUTES.LOGIN);
    } catch (err) {
        // TODO: handle refresh token errors
        console.log(err);
    }
}
