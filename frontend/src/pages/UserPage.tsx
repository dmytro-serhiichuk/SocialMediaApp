import { Link, useLoaderData } from "react-router";
import { useUser } from "../features/auth/hooks";
import type { UserProfile } from "../features/user/types";
import PostList from "../features/posts/components/postList";
import { useState } from "react";
import { apiClient } from "../api/client";
import { ROUTES } from "../constants/routes";

export default function UserPage() {
    const currentUser = useUser();
    const profileUser = useLoaderData<UserProfile>();

    const isOwnProfile = currentUser?.id === profileUser.id;

    const [followersCount, setFollowersCount] = useState<number>(profileUser.followersCount);
    const [isFollowedByMe, setIsFollowedByMe] = useState<boolean>(profileUser.isFollowedByMe);
    const [isBusy, setIsBusy] = useState<boolean>(false);

    const handleFollowButton = async () => {
        try {
            setIsBusy(true);

            await apiClient(`users/${profileUser.id}/follow`, {
                method: isFollowedByMe ? 'DELETE' : 'POST'
            });
            
            setIsFollowedByMe(prev => {
                if (profileUser.isFollowedByMe) {
                    if (prev) setFollowersCount(profileUser.followersCount - 1);
                    else      setFollowersCount(profileUser.followersCount);
                } else {
                    if (prev) setFollowersCount(profileUser.followersCount);
                    else      setFollowersCount(profileUser.followersCount + 1);
                }
                
                return !prev
            });
        } catch (error) {
            if (error instanceof Error) {
                alert(error.message);
            } else {
                alert('Something went wrong');
            }
        } finally {
            setIsBusy(false);
        }
    }

    return(
        <>
            <p>UserName: {profileUser.userName}</p>
            <p>Tag: {profileUser.userTag}</p>

            <p>Owner: {isOwnProfile ? "Yes" : "No"}</p>

            <p>Followers: {followersCount}</p>
            <p>Followings: {profileUser.followingCount}</p>

            {currentUser && !isOwnProfile ? 
                <>
                    <p>Follows Me: {profileUser.isFollowingMe ? "Yes" : "No"}</p>

                    <button onClick={handleFollowButton} disabled={isBusy}>
                        {
                            isFollowedByMe ? 'Unfollow' : 
                            profileUser.isFollowingMe ? 
                            'Follow back' : 
                            'Follow'
                        }
                    </button>
                </> :
                <>
                    <Link to={ROUTES.SAVED_POSTS}>
                        <button>Open saved posts</button>
                    </Link>
                </>
            }
            

            <hr />
            
            <p>Posts:</p>

            <hr />
            
            <PostList endpoint={`users/${profileUser.id}/posts/`} />
        </>
    );
}