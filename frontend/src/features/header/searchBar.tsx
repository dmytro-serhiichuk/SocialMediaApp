import { useEffect, useRef, useState } from "react";
import type { AuthUser } from "../auth/types";
import type { TagSearchResponse } from "../tag/types";
import { Link } from "react-router";
import { getTagPagePath, getUserPagePath } from "../../utils/routerUtils";
import { isUserTagValid, normalizeUserTag } from "../user/validation";
import { apiClient } from "../../api/client";
import { isTagNameValid, normalizeTagName } from "../tag/validation";
import type { UserSearchResponse } from "./types";

export default function SearchBar() {
    const inputRef = useRef<HTMLInputElement | null>(null);
    const [query, setQuery] = useState("");
    const [tagSuggestions, setTagSuggestions] = useState<TagSearchResponse[]>([]);
    const [userSuggestions, setUserSuggestions] = useState<AuthUser[]>([]);

    useEffect(() => {
        if (query.length < 2) return;
        const firstChar = query[0];

        if (firstChar === "@") {
            fetchUsers(query.slice(1));
        } else if (firstChar === "#") {
            fetchTags(query.slice(1));
        } else {
            return;
        }
    }, [query]);

    const handleInputChanged = () => {
        if (inputRef.current) {
            setQuery(inputRef.current.value);
        }
    }

    const fetchUsers = (q: string) => {
        setTagSuggestions([]);
        
        q = normalizeUserTag(q);

        if (!isUserTagValid(q)) {
            setUserSuggestions([]);
            return;
        } else {
            const timeoutId = setTimeout(async () => {
                try {
                    const response = await apiClient<UserSearchResponse[]>(`users/search?query=${q}`);
                    setUserSuggestions(response);
                } catch {
                    return;
                }
            }, 300);

            return () => clearTimeout(timeoutId);
        }
    }

    const fetchTags = (q: string) => {
        setUserSuggestions([]);
        
        q = normalizeTagName(q);

        if (!isTagNameValid(q)) {
            setTagSuggestions([]);
            return;
        } else {
            const timeoutId = setTimeout(async () => {
                try {
                    const response = await apiClient<TagSearchResponse[]>(`tags/search?query=${q}`);
                    setTagSuggestions(response.sort((a, b) => b.useCount - a.useCount));
                } catch {
                    return;
                }
            }, 300);

            return () => clearTimeout(timeoutId);
        }
    }

    const handleLink = () => {
        if (inputRef.current) {
            inputRef.current.value = "";
            setQuery(inputRef.current.value);
            setUserSuggestions([]);
            setTagSuggestions([]);
        }
    }

    return (
        <div>
            <input type="text" onChange={handleInputChanged} ref={inputRef} />

            {tagSuggestions.map(tag => (
                <div>
                    <Link 
                        key={tag.name} 
                        to={getTagPagePath(tag.name)}
                        onClick={handleLink}
                        >
                            #{tag.name}
                    </Link>
                </div>
            ))}
            {userSuggestions.map(user => (
                <div>
                    <Link 
                        key={user.id} 
                        to={getUserPagePath(user.userTag)}
                        onClick={handleLink}
                        >
                            @{user.userTag}
                    </Link>
                </div>
            ))}
        </div>
    );
}