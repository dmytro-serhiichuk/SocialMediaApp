import { InfinityScroll, type InfiniteScrollStatus } from "../../pagination/inifinityScroll";
import type { Comment } from "../types";
import { ReplyEntry } from "./replyEntry";

type ReplyEntryListProps = {
    postId: number;
    handleReplyCreated: (reply: Comment) => void;
    replies: Comment[];
    status: InfiniteScrollStatus;
    onLoadMore: () => void;
    scrollRootRef?: React.RefObject<HTMLElement | null>;
}

export function ReplyEntryList({
    postId, handleReplyCreated, replies, status, onLoadMore, scrollRootRef
}: ReplyEntryListProps) {
    return (
        <div className="reply-list">
            {replies.map(reply => (
                <ReplyEntry
                    key={reply.id}
                    reply={reply}
                    postId={postId}
                    handleReplyCreated={handleReplyCreated} />
            ))}

            <InfinityScroll
                status={status}
                onLoadMore={onLoadMore}
                rootRef={scrollRootRef} />
        </div>
    );
}