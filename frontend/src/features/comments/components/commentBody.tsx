import { Link } from "react-router";
import { useUser } from "../../auth/hooks";
import { getUserPagePath } from "../../../utils/routerUtils";
import type { Comment } from "../types";

type CommentBodyProps = {
    comment: Comment;
    isReply: boolean;
    onReplyClick: () => void;
}

export function CommentBody({ comment, isReply, onReplyClick }: CommentBodyProps) {
    const currentUser = useUser();

    return (
        <>
            <Link to={getUserPagePath(comment.user.userTag)}>
                <p>{comment.user.userName}</p>
                <p>{comment.user.userTag}</p>
            </Link>

            <p>{new Date(comment.createdAt).toISOString()}</p>
            {comment.updatedAt && (
                <p><i>edited</i> {new Date(comment.updatedAt).toISOString()}</p>
            )}

            <p>
                {isReply && comment.replyToUserTag && (
                    <Link to={getUserPagePath(comment.replyToUserTag)}>@{comment.replyToUserTag} </Link>
                )}
                {comment.content}
            </p>

            <p>Likes: 10</p>

            {currentUser && (
                <>
                    <button>Like</button>
                    <button onClick={onReplyClick}>Reply</button>
                </>
            )}
        </>
    );
}