import type { Comment } from "../types";
import CommentInput from "./commentInput";
import { useState } from "react";
import { useUser } from "../../auth/hooks";
import { CommentBody } from "./commentBody";

type ReplyEntryProps = {
    reply: Comment;
    postId: number;
    handleReplyCreated: (reply: Comment) => void;
}

export function ReplyEntry({ reply, postId, handleReplyCreated }: ReplyEntryProps) {
    const currentUser = useUser();
    const [replyInputVisible, setReplyInputVisible] = useState(false);

    return (
        <div className="reply">
            <CommentBody
                comment={reply}
                isReply={true}
                onReplyClick={() => setReplyInputVisible(prev => !prev)} />

            {currentUser && replyInputVisible && (
                <>
                    <hr />
                    <CommentInput
                        postId={postId}
                        onCommentCreated={handleReplyCreated}
                        repliedCommentId={reply.id} />
                </>
            )}

            <hr />
        </div>
    );
}