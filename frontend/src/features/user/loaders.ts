import { redirect, type LoaderFunctionArgs } from "react-router";
import { apiClient } from "../../api/client";
import type { UserProfile } from "./types";
import { authStore } from "../auth/store";
import { getCurrentUser } from "../auth/api";
import { getUserPagePath } from "../../utils/routerUtils";

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

export async function profileLoader() {
    const userTag = authStore.getUser()?.userTag;
    if (userTag) {
        return redirect(getUserPagePath(userTag));
    }

    const user = await getCurrentUser();
    
    if (user) {
        authStore.setUser(user);
        return redirect(getUserPagePath(user.userTag));
    }
}