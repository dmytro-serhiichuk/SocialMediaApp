import { InfinityScroll, type InfinityScrollStatus } from "../../pagination/infinityScroll";
import type { Comment } from "../types";
import CommentEntry from "./commentEntry";

type CommentEntryListProps = {
    postId: number;
    commentsCount: number;
    comments: Comment[];
    status: InfinityScrollStatus;
    onLoadMore: () => void;
    scrollRootRef: React.RefObject<HTMLElement | null>;
    onReplyCreated: (comment: Comment) => void;
    updateComment: (id: number, updater: (item: Comment) => Comment) => void;
}

export default function CommentEntryList({
    postId, commentsCount, comments, status, onLoadMore, scrollRootRef, onReplyCreated, updateComment
}: CommentEntryListProps) {
    return (
        <div className="comments-list">
            <h3>Comments {commentsCount}:</h3>
            <hr />

            {comments.map(comment => (
                <CommentEntry
                    key={comment.id}
                    comment={comment}
                    postId={postId}
                    scrollRootRef={scrollRootRef}
                    onReplyCreated={onReplyCreated}
                    updateComment={updateComment} />
            ))}

            <InfinityScroll
                status={status}
                onLoadMore={onLoadMore}
                rootRef={scrollRootRef} />
        </div>
    );
}