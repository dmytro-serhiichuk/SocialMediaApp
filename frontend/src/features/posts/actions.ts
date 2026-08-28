import { redirect, type ActionFunctionArgs } from "react-router";
import { apiClient, ApiError } from "../../api/client";
import { ROUTES } from "../../constants/routes";

export async function createPostAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    try {
        await apiClient('posts/create', {
            method: "POST",
            body: formData
        });

        return redirect(ROUTES.HOME);
    } catch (err) {
        if (err instanceof ApiError) {
            return {
                error: err.message,
                validationErrors: err.validationErrors
            };
        }
        throw err;
    }
}