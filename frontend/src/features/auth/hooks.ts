import type { AuthUser } from "./types";
import { useSyncExternalStore } from "react";
import { authStore } from "./store";

export function useUser(): AuthUser | null {
    return useSyncExternalStore(authStore.subscribe, authStore.getUser, authStore.getUser);
}