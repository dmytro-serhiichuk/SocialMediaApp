import { Link } from "react-router";
import { getUserPagePath } from "../../../utils/routerUtils";
import type { Comment } from "../types";

export default function CommentComponent({ comment }: { comment: Comment }) {
    return (
        <div className="comment">
            <Link to={getUserPagePath(comment.user.userTag)}>
                <p>{comment.user.userName}</p>
                <p>{comment.user.userTag}</p>
            </Link>

            <p>{new Date(comment.createdAt).toISOString()}</p>
            {comment.updatedAt && (
                <p><i>edited</i> {new Date(comment.updatedAt).toISOString()}</p>
            )}

            <p>{comment.content}</p>

            <button>Like 10</button>
            <button>Reply</button>

            <hr />
        </div>
    );
}