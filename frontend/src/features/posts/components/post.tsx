import { Link } from "react-router";
import type { Post } from "../types";
import { getUserPagePath } from "../../../utils/routerUtils";

export default function PostComponent({ post }: { post: Post }) {
    return (
        <div className="post-container">
            <Link to={getUserPagePath(post.user.userTag)}>
                <p>{post.user.userName}</p>
                <p>{post.user.userTag}</p>
            </Link>

            <p>{new Date(post.createdAt).toDateString()}</p>

            <div>
                <p>{post.content}</p>
            </div>
        </div>
    );
}