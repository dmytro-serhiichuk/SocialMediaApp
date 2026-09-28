import { useState } from "react";
import type { UserResponse } from "../types";
import { Link } from "react-router";
import { getUserPagePath } from "../../../utils/routerUtils";
import { useToggleApiRequest } from "../../../api/hooks";

export default function UserComponent({user}: {user: UserResponse}) {
    const [isFollowedByMe, setIsFollowedByMe] = useState<boolean>(user.isFollowedByMe);
    const [isBusy, setIsBusy] = useState<boolean>(false);
    const { toggleApiRequest } = useToggleApiRequest(setIsBusy);

    const handleClick = async () => {
        toggleApiRequest(
            `users/${user.id}/follow`, 
            isFollowedByMe ? 'DELETE' : 'POST', 
            () => setIsFollowedByMe(prev => !prev));
    }

    return (
        <div className="user-container">
            <Link to={getUserPagePath(user.userTag)}>
                <p>{user.userName}</p>
                <p>@{user.userTag}</p>
            </Link>
            
            <button disabled={isBusy} onClick={handleClick}>
                {isFollowedByMe 
                    ? "Unfollow"
                    : user.isFollowingMe
                        ? "Follow back"
                        : "Follow"}
            </button>
        </div>
    );
}