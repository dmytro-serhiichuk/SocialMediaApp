import type { LoaderFunctionArgs } from "react-router";
import { apiClient } from "../../api/client";
import type { UserProfile } from "./types";
import { authStore } from "../auth/store";

export async function userLoader({ params }: LoaderFunctionArgs) {
    if (!params.userTag) {
        throw new Response("User tag is required", { status: 400 });
    }

    let currentUser = authStore.getUser(); 
    const user = await apiClient<UserProfile>(`users/${params.userTag}`);

    if (currentUser?.id === user.id) {
        authStore.updateUser({
            userName: user.userName,
            userTag: user.userTag
        });
    }

    return user;
}