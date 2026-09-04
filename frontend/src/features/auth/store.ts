import type { User } from "./types";

type Listener = () => void;

let currentUser: User | null = null;
const listeners = new Set<Listener>();

function notify() {
    listeners.forEach(listener => listener());
}

export const authStore = {
    getUser(): Readonly<User | null> {
        return currentUser;
    },

    setUser(user: User | null): void {
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

    updateUser(update: Partial<User>) {
        if (!currentUser) return;

        currentUser = {
            ...currentUser,
            ...update
        };

        notify();
    }
}