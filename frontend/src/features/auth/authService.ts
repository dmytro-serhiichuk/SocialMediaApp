import type { User } from "./types";

// TODO: fetch data from api
export async function getUser(): Promise<User | null> {
    return Promise.resolve({
        id: 1,
        Email: "example@example.com"
    });
}