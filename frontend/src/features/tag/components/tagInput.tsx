import { useEffect, useRef, useState } from "react";
import type { TagSearchResponse } from "../types";
import { apiClient } from "../../../api/client";

export interface TagInputProps {
    tags: string[];
    onChange: (prev: string[]) => void;
};

export function TagInput({tags, onChange}: TagInputProps) {
    const inputRef = useRef<HTMLInputElement | null>(null);
    const [query, setQuery] = useState("");
    const [suggestions, setSuggestions] = useState<TagSearchResponse[]>([]);

    const regex = /^[a-z0-9_]+$/i;

    const handleRemove = (tag: string) => {
        onChange(tags.filter(t => t !== tag));
    }

    const handleAdd = () => {
        const input = inputRef.current;

        if (!input) return;

        const value = input.value.trim().toLocaleLowerCase();
        if (value.length > 32 || !regex.test(value)) return;

        if (!tags.includes(value)) {
            onChange([...tags, value]);
        }
    }

    useEffect(() => {
        const q = query.trim().toLocaleLowerCase();

        if (q.length <= 2 || !regex.test(q)) {
            setSuggestions([]);
            return;
        } else {
            const timeoutId = setTimeout(async () => {
                try {
                    const response = await apiClient<TagSearchResponse[]>(`tags/?query=${q}`);
                    setSuggestions(response.sort((a, b) => b.useCount - a.useCount));
                } catch {
                    return;
                }
            }, 300);

            return () => clearTimeout(timeoutId);
        }
    }, [query]);

    const handleInputChanged = () => {
        if (inputRef.current) {
            setQuery(inputRef.current.value);
        }
    }
    
    return (
        <div>
            {tags.map((tag, i) => (
                <div key={i}>
                    <p>{tag}</p>
                    <button type="button" onClick={() => handleRemove(tag)}>Remove</button>
                </div>
            ))}

            {tags.length < 5 &&
                <>
                    <input type="text" ref={inputRef} maxLength={32} onChange={handleInputChanged} />
                    {suggestions.length > 0 && <p>Suggestions:</p>}
                    {suggestions.map(s => (
                        <p>{s.name} - used {s.useCount} times</p>
                    ))}
                    <button type="button" onClick={handleAdd}>Add Tag</button>
                </>
            }
        </div>
    );
}