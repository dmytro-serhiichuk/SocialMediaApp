import { getUser } from "./api";

export async function authLoader() {
    return getUser();
}