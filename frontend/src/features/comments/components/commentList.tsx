import { InfinityScroll, type InfiniteScrollStatus } from "../../pagination/inifinityScroll";
import type { Comment } from "../types";
import CommentComponent from "./comment";

type CommentListProps = {
    comments: Comment[];
    status: InfiniteScrollStatus;
    onLoadMore: () => void;
    scrollRootRef?: React.RefObject<HTMLElement | null>;
}

export default function CommentList({ comments, status, onLoadMore, scrollRootRef }: CommentListProps) {
    return (
        <div className="comments-list">
            <h3>Comments:</h3>
                    
            <hr />

            {comments.map(comment => (
                <CommentComponent comment={comment} key={comment.id} />
            ))}

            <InfinityScroll 
                status={status}
                onLoadMore={onLoadMore}
                rootRef={scrollRootRef} />
        </div>
    );
}