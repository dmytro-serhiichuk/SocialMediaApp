import type { LoaderFunctionArgs } from "react-router";
import { apiClient } from "../../api/client";
import type { PostEditInfo } from "./types";

export async function editPostLoader({ params }: LoaderFunctionArgs) {
    if (!params.postId) {
        throw new Response("Post id is required", { status: 400 });
    }

   return await apiClient<PostEditInfo>(`posts/${params.postId}/edit`);
}