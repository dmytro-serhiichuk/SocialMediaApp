import { Link, useLoaderData } from "react-router";
import { useUser } from "../features/auth/hooks";
import type { UserProfile } from "../features/user/types";
import PostList from "../features/posts/components/postList";
import { useEffect, useState } from "react";
import { ROUTES } from "../constants/routes";
import { getUserFollowersPagePath, getUserFollowingPagePath } from "../utils/routerUtils";
import { useToggleApiRequest } from "../api/hooks";

export default function UserPage() {
    const currentUser = useUser();
    const profileUser = useLoaderData<UserProfile>();

    const isOwnProfile = currentUser?.id === profileUser.id;

    const [followersCount, setFollowersCount] = useState<number>(profileUser.followersCount);
    const [isFollowedByMe, setIsFollowedByMe] = useState<boolean>(profileUser.isFollowedByMe);
    const [isBusy, setIsBusy] = useState<boolean>(false);
    const { toggleApiRequest } = useToggleApiRequest(setIsBusy);

    const handleFollowButton = async () => {
        toggleApiRequest(
            `users/${profileUser.id}/follow`, 
            isFollowedByMe ? 'DELETE' : 'POST', 
            () => {
                setIsFollowedByMe(prev => !prev);
                setFollowersCount(prev => isFollowedByMe ? prev - 1 : prev + 1);
            });
    }

    useEffect(() => {
        setFollowersCount(profileUser.followersCount);
        setIsFollowedByMe(profileUser.isFollowedByMe);
    }, [profileUser.id]);

    return(
        <>
            {isOwnProfile && <Link to={ROUTES.SETTINGS}>Settings</Link>}

            <p>UserName: {profileUser.userName}</p>
            <p>Tag: {profileUser.userTag}</p>

            <p>Owner: {isOwnProfile ? "Yes" : "No"}</p>
            
            <Link to={getUserFollowersPagePath(profileUser.userTag)}>
                <button>
                    Followers: {followersCount}
                </button>
            </Link>
            <Link to={getUserFollowingPagePath(profileUser.userTag)}>
                <button>
                    Followings: {profileUser.followingCount}
                </button>
            </Link>

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
            
            <PostList
                key={profileUser.id} 
                endpoint={`users/${profileUser.id}/posts/`} />
        </>
    );
}