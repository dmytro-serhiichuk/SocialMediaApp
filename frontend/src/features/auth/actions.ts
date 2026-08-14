import { redirect, type ActionFunctionArgs } from "react-router";
import { apiClient, ApiError } from "../../api/client";
import { ROUTES } from "../../constants/routes";

export async function registerAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    try {
        await apiClient('auth/register', {
            method: "POST",
            body: JSON.stringify({
                userName: formData.get("userName"),
                userTag: formData.get("userTag"),
                email: formData.get("email"),
                password: formData.get("password"),
            })
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

    try {
        await apiClient('auth/login', {
            method: "POST",
            body: JSON.stringify({
                login: formData.get("login"),
                password: formData.get("password"),
            })
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