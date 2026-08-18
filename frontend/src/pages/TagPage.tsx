import { useLoaderData } from "react-router";
import type { TagFullResponse } from "../features/tag/types";
import PostList from "../features/posts/components/postList";

export default function TagPage() {
    const tag = useLoaderData<TagFullResponse>();

    return (
        <>
            <h1># {tag.name}</h1>
            <h2>Was used: {tag.useCount} times</h2>

            <hr />

            <PostList endpoint={`tags/${tag.id}/posts/`} key={tag.id} />
        </>
    );
}