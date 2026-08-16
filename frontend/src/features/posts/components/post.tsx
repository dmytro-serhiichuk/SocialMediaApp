import type { Post } from "../types";

export default function PostComponent({ post }: { post: Post }) {
    return (
        <div className="post-container">
            <div>
                <p>{post.user.userName}</p>
                <p>{post.user.userTag}</p>
            </div>

            <p>{new Date(post.createdAt).toDateString()}</p>

            <div>
                <p>{post.content}</p>
            </div>
        </div>
    );
}