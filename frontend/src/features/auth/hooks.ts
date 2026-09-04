import type { User } from "./types";
import { useSyncExternalStore } from "react";
import { authStore } from "./store";

export function useUser(): User | null {
    return useSyncExternalStore(authStore.subscribe, authStore.getUser, authStore.getUser);
}