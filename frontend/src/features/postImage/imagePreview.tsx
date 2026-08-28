import { useEffect, useState } from "react";

export default function ImagePreview({ file, onRemove }: { file: File, onRemove: () => void }) {
    const [url, setUrl] = useState<string>();

    useEffect(() => {
        const objUrl = URL.createObjectURL(file);

        setUrl(objUrl);

        return () => {
            URL.revokeObjectURL(objUrl);
        };
    }, [file]);

    if (!url) return null;

    return (
        <div className="image-input-item">
            <img src={url} alt="" />
            <button type="button" onClick={onRemove}>Remove</button>
        </div>
    )
}