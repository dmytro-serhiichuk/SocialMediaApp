import { useRef } from "react";

export interface TagInputProps {
    tags: string[];
    onChange: (prev: string[]) => void;
};

export function TagInput({tags, onChange}: TagInputProps) {
    const inputRef = useRef<HTMLInputElement | null>(null);

    const regex = /^[a-z0-9_]+$/i;

    const handleRemove = (tag: string) => {
        onChange(tags.filter(t => t !== tag));
    }

    const handleAdd = () => {
        const input = inputRef.current;

        if (!input) return;

        const value = input.value.trim().toLocaleLowerCase();
        if (!regex.test(value)) return;

        if (!tags.includes(value)) {
            onChange([...tags, value]);
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
                {/* TODO: get list of existing similar tags from API */}
                    <input type="text" ref={inputRef} maxLength={32} />
                    <button type="button" onClick={handleAdd}>Add Tag</button>
                </>
            }
        </div>
    );
}