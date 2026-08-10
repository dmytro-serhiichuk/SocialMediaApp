import { getUser } from "./authService";

export async function authLoader() {
    return await getUser();
}