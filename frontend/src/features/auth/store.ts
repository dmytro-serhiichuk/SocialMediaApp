import type { AuthUser } from "./types";

type Listener = () => void;

let currentUser: AuthUser | null = null;
const listeners = new Set<Listener>();

function notify() {
    listeners.forEach(listener => listener());
}

export const authStore = {
    getUser(): Readonly<AuthUser | null> {
        return currentUser;
    },

    setUser(user: AuthUser | null): void {
        currentUser = user;
        notify();
    },

    clearUser(): void {
        currentUser = null;
        notify();
    },

    subscribe(listener: Listener): () => void {
        listeners.add(listener);
        return () => listeners.delete(listener);
    },

    updateUser(update: Partial<AuthUser>) {
        if (!currentUser) return;

        currentUser = {
            ...currentUser,
            ...update
        };

        notify();
    }
}