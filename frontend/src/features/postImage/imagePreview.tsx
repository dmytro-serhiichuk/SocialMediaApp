import { useEffect, useState } from "react";
import { getFullImageUrl } from "../image/utils";
import type { InputImageInfo } from "./types";

type ImagePreviewProps = {
    image: InputImageInfo,
}

export default function ImagePreview({ image }: ImagePreviewProps) {
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
        <img src={imageUrl} alt="" />
    )
}