import { getCurrentUser } from "./api";

export async function authLoader() {
    return getCurrentUser();
}