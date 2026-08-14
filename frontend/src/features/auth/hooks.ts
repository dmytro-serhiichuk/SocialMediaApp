import { useRouteLoaderData } from "react-router";
import type { User } from "./types";
import { ROOT_ID } from "../../constants/routes";

export function useUser(): User | null {
    return useRouteLoaderData<User | null>(ROOT_ID) ?? null;
}

export function useRequiredUser(): User {
    const user = useRouteLoaderData<User | null>(ROOT_ID);

    if (!user) throw new Error("useRequiredUser called without an authenticated user");

    return user;
}