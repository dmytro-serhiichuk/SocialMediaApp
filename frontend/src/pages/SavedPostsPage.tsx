import PostList from "../features/posts/components/postList";

export default function SavedPostsPage() {
    return(
        <>
            <p>Saved:</p>
            
            <hr />
            
            <PostList endpoint={`posts/saved/`} />
        </>
    )
}