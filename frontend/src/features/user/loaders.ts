import type { LoaderFunctionArgs } from "react-router";
import { apiClient } from "../../api/client";
import type { UserProfile } from "./types";

export async function userLoader({ params }: LoaderFunctionArgs) {
    if (!params.userTag) {
        throw new Response("User tag is required", { status: 400 });
    }

    return apiClient<UserProfile>(`users/${params.userTag}`);
}