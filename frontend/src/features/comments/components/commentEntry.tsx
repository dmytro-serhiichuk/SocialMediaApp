import type { Comment } from "../types";
import { useState } from "react";
import { useReplies } from "../hooks";
import CommentInput from "./commentInput";
import { useUser } from "../../auth/hooks";
import { CommentBody } from "./commentBody";
import { ReplyEntryList } from "./replyEntryList";
import { toggleLike } from "../utils";

type CommentEntryProps = {
    comment: Comment;
    postId: number;
    scrollRootRef: React.RefObject<HTMLElement | null>;
    onReplyCreated: (comment: Comment) => void;
    updateComment: (id: number, updater: (item: Comment) => Comment) => void;
}

export default function CommentEntry({ comment, postId, scrollRootRef, onReplyCreated, updateComment }: CommentEntryProps) {
    const currentUser = useUser();
    const [repliesCount, setRepliesCount] = useState(comment.repliesCount);
    const [repliesVisible, setRepliesVisible] = useState(false);
    const [replyInputVisible, setReplyInputVisible] = useState(false);
    const { replies, status, loadMore, addReply, updateReply } = useReplies(postId, comment.id);

    const handleReplyCreated = (reply: Comment) => {
        onReplyCreated(reply);
        setRepliesCount(prev => prev + 1);
        if (repliesVisible) {
            addReply(reply);
        }
    }

    return (
        <div className="comment">
            <CommentBody
                comment={comment}
                isReply={false}
                onReplyClick={() => setReplyInputVisible(prev => !prev)}
                onLikeToggle={(c) => updateComment(c.id, toggleLike)} />

            {repliesCount > 0 && (
                <button onClick={() => setRepliesVisible(prev => !prev)}>
                    See replies: {repliesCount}
                </button>
            )}

            <hr />

            {currentUser && replyInputVisible && (
                <>
                    <CommentInput
                        postId={postId}
                        onCommentCreated={handleReplyCreated}
                        repliedCommentId={comment.id} />
                    <hr />
                </>
            )}

            {repliesVisible && (
                <ReplyEntryList
                    postId={postId}
                    handleReplyCreated={handleReplyCreated}
                    replies={replies}
                    status={status}
                    onLoadMore={loadMore}
                    scrollRootRef={scrollRootRef}
                    updateReply={updateReply} />
            )}
        </div>
    );
}