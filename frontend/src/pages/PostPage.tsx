import { useLoaderData } from "react-router";
import PostExtended from "../features/posts/components/postExtended";
import { postPageLoader } from "../features/posts/loaders";

export default function PostPage() {
    const post = useLoaderData<typeof postPageLoader>();

    return (
        <div className="post-page-container">
            <PostExtended post={post} />
        </div>
    );
}