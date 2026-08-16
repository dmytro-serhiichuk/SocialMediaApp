import { useLoaderData } from "react-router";
import { useUser } from "../features/auth/hooks";
import type { UserProfile } from "../features/user/types";

export default function UserPage() {
    const currentUser = useUser();
    const profileUser = useLoaderData<UserProfile>();

    const isOwnProfile = currentUser?.id === profileUser.id;

    return(
        <>
            <p>UserName: {profileUser.userName}</p>
            <p>Tag: {profileUser.userTag}</p>

            <p>Owner: {isOwnProfile ? "Yes" : "No"}</p>
        </>
    );
}