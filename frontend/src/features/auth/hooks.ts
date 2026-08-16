import { useRouteLoaderData } from "react-router";
import type { User } from "./types";
import { ROOT_ID } from "../../constants/routes";

export function useUser(): User | null {
    return useRouteLoaderData<User | null>(ROOT_ID) ?? null;
}