import { redirect, type ActionFunctionArgs } from "react-router";
import { getFormString, getFormStrings } from "../../utils/formDataUtils";
import type { PostForm } from "./types";
import { apiClient, ApiError } from "../../api/client";
import { ROUTES } from "../../constants/routes";

export async function createPostAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    const requestBody: PostForm = {
        content: getFormString(formData, "content"),
        tags:    getFormStrings(formData, "tags")
    }

    try {
        await apiClient('posts/create', {
            method: "POST",
            body: JSON.stringify(requestBody)
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