import { useState } from "react";
import { InfinityScroll, type InfinityScrollStatus } from "../../pagination/infinityScroll";
import type { UserResponse } from "../types";
import type { PaginationResponse } from "../../pagination/types";
import { apiClient } from "../../../api/client";
import UserPlaceholder from "./userPlaceholder";
import UserComponent from "./user";

export type UserListEndpoint = {
    endpoint: string
};

export default function UserList({endpoint}: UserListEndpoint) {
    const [state, setStatus]  = useState<InfinityScrollStatus>("idle");
    const [cursor, setCursor] = useState<string | null>(null);
    const [users, setUsers]   = useState<UserResponse[]>([]);
    
    const loadMore = async () => {
        setStatus("loading");

        try {
            const response = await apiClient<PaginationResponse<UserResponse>>(cursor ? endpoint += `?cursor=${cursor}` : endpoint);

            setUsers(prev => [...prev, ...response.items]);
            setCursor(response.nextCursor);
            setStatus(response.hasMore ? "idle" : "finished");
        } catch {
            setStatus("error");
        }
    }

    return (
        <div className="uses-list">
            {users.map(user => (
                <UserComponent user={user} key={user.id} />
            ))}

            <InfinityScroll 
                status={state}
                onLoadMore={loadMore}
                loadingPlaceholer={<UserPlaceholder />}/>
        </div>
    )
}