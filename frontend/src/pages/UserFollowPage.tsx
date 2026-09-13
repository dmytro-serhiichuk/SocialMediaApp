import { useLoaderData } from "react-router";
import type { UserFollowLoaderData } from "../features/user/loaders";
import UserList from "../features/user/components/userList";

export default function UserFollowPage() {
    const data = useLoaderData<UserFollowLoaderData>();
    
    return (
        <>
            <h3>{data.page}:</h3>

            <UserList 
                key={data.userTag}
                endpoint={`users/${data.userTag}/${data.page === "Followers" ? "followers" : "following"}/`} />
        </>
    );
}