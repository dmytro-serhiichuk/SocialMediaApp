import { useState } from "react";
import PostList from "../features/posts/components/postList";
import { useUser } from "../features/auth/hooks";

export type FeedOption =
    | "recent"
    | "following";

const feedEndpoints: Record<FeedOption, string> = {
    recent: "posts",
    following: "posts/following",
};


export default function HomePage() {
    const user = useUser();

    const [feedOption, setFeedOption] = useState<FeedOption>("recent");

    return (
        <>
            <h1>Home page</h1>

            {user &&
                <>
                    <button onClick={() => setFeedOption("recent")}>
                        Recent
                    </button>
                    <button onClick={() => setFeedOption("following")}>
                        Following
                    </button>
                </>
            }

            <PostList 
                endpoint={feedEndpoints[feedOption]} 
                key={feedOption}
            />
        </>
    );
}