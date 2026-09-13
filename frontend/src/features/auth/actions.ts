import { redirect, type ActionFunctionArgs } from "react-router";
import { apiClient, ApiError } from "../../api/client";
import type { LoginRequest, RegisterCompletionRequest, RegisterRequest, RequestPasswordResetRequest, ResetPasswordRequest, AuthUser } from "./types";
import { getFormString } from "../../utils/formDataUtils";
import { authStore } from "./store";
import { logout } from "./api";
import { ROUTES } from "../../constants/routes";

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

    const errorsStatuses = [400, 401, 403];

    try {
        const user = await apiClient<AuthUser>('auth/login', {
            method: "POST",
            body: JSON.stringify(loginRequest)
        });

        authStore.setUser(user);
    } catch (err) {
        if (err instanceof ApiError && errorsStatuses.includes(err.status)) {
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
        const user = await apiClient<AuthUser>('auth/register/complete', {
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

export async function requestPasswordResetAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    const registerRequest: RequestPasswordResetRequest = {
        email: getFormString(formData, "email"),
        requestType: "Reset"
    };

    try {
        await apiClient('auth/password-reset/request', {
            method: "POST",
            body: JSON.stringify(registerRequest)
        });

        return {
            success: true
        };
    } catch (err) {
        if (err instanceof ApiError && err.status === 400) {
            return {
                success: false,
                error: err.message
            };
        }
        throw err;
    }
}

export async function completePasswordResetAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    try {
        return await resetPassword(formData, "auth/password-reset/confirm");
    } catch (error) {
        if (error instanceof ApiError && error.status === 400) {
            return {
                error: error.message,
                validationErrors: error.validationErrors
            };
        }
        throw error;
    }
}

export async function recoverAccountAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    const expectedStatuses = [400, 409];

    try {
        return await resetPassword(formData, "auth/acccount-recovery/complete");
    } catch (error) {
        if (error instanceof ApiError && expectedStatuses.includes(error.status)) {
            return {
                error: error.message,
                validationErrors: error.validationErrors,
                recoveryNotAvailable: error.status === 409
            };
        }
        throw error;
    }
}

async function resetPassword(formData: FormData, endpoint: string) {
    const registerRequest: ResetPasswordRequest = {
        token: getFormString(formData, "token"),
        newPassword: getFormString(formData, "new-password")
    };

    await apiClient(endpoint, {
        method: "POST",
        body: JSON.stringify(registerRequest)
    });

    return redirect(ROUTES.LOGIN);
}
