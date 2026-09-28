import { Link } from "react-router";
import { useUser } from "../../auth/hooks";
import { getUserPagePath } from "../../../utils/routerUtils";
import type { Comment } from "../types";
import { useState } from "react";
import { useToggleApiRequest } from "../../../api/hooks";

type CommentBodyProps = {
    comment: Comment;
    isReply: boolean;
    onReplyClick: () => void;
    onLikeToggle: (comment: Comment) => void;
}

export function CommentBody({ comment, isReply, onReplyClick, onLikeToggle }: CommentBodyProps) {
    const currentUser = useUser();
    const [isBusy, setIsBusy] = useState<boolean>(false);
    const { toggleApiRequest } = useToggleApiRequest(setIsBusy);

    const handleLikePressed = async () => {
        toggleApiRequest(
            `comments/${comment.id}/likes`, 
            comment.isLikedByMe ? 'DELETE' : 'POST', 
            () => onLikeToggle(comment));
        }

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
            
            <p>Likes: {comment.likesCount}</p>
            {currentUser && 
                <> 
                    <button onClick={handleLikePressed} disabled={isBusy}>
                        {comment.isLikedByMe ? "Unlike" : "Like"}
                    </button>
                    <button onClick={onReplyClick}>
                        Reply
                    </button>
                </>
            }
        </>
    );
}