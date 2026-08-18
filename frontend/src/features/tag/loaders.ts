import type { LoaderFunctionArgs } from "react-router";
import { apiClient } from "../../api/client";
import type { TagFullResponse } from "./types";

export function tagLoader({ params }: LoaderFunctionArgs) {
    if (!params.tagName) {
        throw new Response("Tag name is required", { status: 400 });
    }

    return apiClient<TagFullResponse>(`tags/${params.tagName}`);
}