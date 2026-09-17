import { useEffect, useState } from "react";
import { getFullImageUrl } from "../image/utils";
import type { InputImageInfo } from "./types";

type ImagePreviewProps = {
    image: InputImageInfo,
    onRemove: () => void
}

export default function ImagePreview({ image, onRemove }: ImagePreviewProps) {
    const [url, setUrl] = useState<string>();

    useEffect(() => {
        if (image.type !== "new") return;

        const objUrl = URL.createObjectURL(image.file);

        setUrl(objUrl);

        return () => {
            URL.revokeObjectURL(objUrl);
        };
    }, [image]);

    const imageUrl = image.type === "existing" ? getFullImageUrl(image.url) : url

    if (!imageUrl) return null;

    return (
        <div className="image-input-item">
            <img src={imageUrl} alt="" />
            <button type="button" onClick={onRemove}>Remove</button>
        </div>
    )
}