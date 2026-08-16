import { useState } from "react"
import { InfinityScroll, type InfiniteScrollStatus } from "../../pagination/inifinityScroll";
import type { PaginationResponse } from "../../pagination/types";
import type { Post } from "../types";
import { apiClient } from "../../../api/client";
import PostComponent from "./post";
import PostPlaceholder from "./postPlaceholder";

export type PostListEndpoint = {
    endpoint: string
};

export default function PostList({endpoint}: PostListEndpoint) {
    const [state, setStatus]  = useState<InfiniteScrollStatus>("idle");
    const [cursor, setCursor] = useState<string | null>(null);
    const [posts, setPosts]   = useState<Post[]>([]);
    
    const loadMore = async () => {
        setStatus("loading");

        try {
            const response = await apiClient<PaginationResponse<Post>>(cursor ? endpoint += `?cursor=${cursor}` : endpoint);

            setPosts(prev => [...prev, ...response.items]);
            setCursor(response.nextCursor);
            setStatus(response.hasMore ? "idle" : "finished");
        } catch {
            setStatus("error");
        }
    }

    return (
        <div className="post-list">
            {posts.map(post => (
                <PostComponent post={post} key={post.id} />
            ))}

            <InfinityScroll 
                status={state}
                onLoadMore={loadMore}
                loadingPlaceholer={<PostPlaceholder />}/>
        </div>
    )
}