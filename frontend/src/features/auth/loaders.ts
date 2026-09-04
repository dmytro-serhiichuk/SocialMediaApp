import { getCurrentUser } from "./api";
import { authStore } from "./store";

export async function authLoader() {
    const user = await getCurrentUser();
    authStore.setUser(user);
}