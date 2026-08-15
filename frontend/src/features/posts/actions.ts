import { redirect, type ActionFunctionArgs } from "react-router";
import { getFormString } from "../../utils/formDataUtils";
import type { PostForm } from "./types";
import { apiClient, ApiError } from "../../api/client";
import { ROUTES } from "../../constants/routes";

export async function createPostAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();

    const requestBody: PostForm = {
        content: getFormString(formData, "content")
    }

    try {
        await apiClient('post/create', {
            method: "POST",
            body: JSON.stringify(requestBody)
        });

        return redirect(ROUTES.PROFILE);
    } catch (err) {
        if (err instanceof ApiError) {
            return {
                error: err.message
            };
        }
        throw err;
    }
}