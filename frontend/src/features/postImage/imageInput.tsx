import { useRef, useState } from "react";
import ImagePreview from "./imagePreview";

export default function ImageInput() {
    const inputRef = useRef<HTMLInputElement>(null);
    
    const [images, setImages] = useState<File[]>([]);
    const [error, setError] = useState<string>();

    const handleImagesChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const files = Array.from(event.target.files ?? []);

        if (files.length === 0) return;

        setError("");

        if (files.length + images.length > MAX_IMAGES) {
            setError(`You can add up to ${MAX_IMAGES} images.`);
            return;
        }

        const validFiles: File[] = [];
        const errors: string[] = [];

        for (const file of files) {
            const err = await validateImage(file);

            if (err) errors.push(`${file.name}: ${err}`);
            else     validFiles.push(file);
        }

        setImages(prev => [...prev, ...validFiles]);

        if (errors.length > 0) setError(errors.join("\n"));
    };

    const handleRemoveImage = (index: number) => {
        setImages(prev => prev.filter((_, i) => i !== index));
    }

    const handleAddButtonClicked = () => {
        inputRef.current?.click();
    }

    return (
        <>
            <input
                ref={inputRef}
                type="file"
                accept="image/jpeg,image/png,image/webp"
                multiple
                hidden
                onChange={handleImagesChange}
                name="images"/>

            <button type="button" onClick={handleAddButtonClicked}>Add photos</button>

            {error && <p>{error}</p>}

            {images.map((image, index) => (
                <ImagePreview
                    key={`${image.name}-${image.lastModified}-${index}`}
                    file={image}
                    onRemove={() => handleRemoveImage(index)} />
            ))}
        </>
    );
}


const MAX_IMAGES = 10;
const MAX_FILE_SIZE_MB = 24;
const MAX_FILE_SIZE = MAX_FILE_SIZE_MB * 1024 * 1024; // 24 MB

const ALLOWED_TYPES = new Set([
    "image/jpeg",
    "image/png",
    "image/webp"
]);

const MIN_WIDTH = 200;
const MIN_HEIGHT = 200;

const MAX_WIDTH = 10000;
const MAX_HEIGHT = 10000;

const MIN_ASPECT_RATIO = 1 / 4;
const MAX_ASPECT_RATIO = 4;

function validateFile(file: File): string | null {
    if (!ALLOWED_TYPES.has(file.type)) {
        return "Only JPEG, PNG and WebP images are allowed.";
    }

    if (file.size > MAX_FILE_SIZE) {
        return `Image must be smaller than ${MAX_FILE_SIZE_MB} MB.`;
    }

    return null;
}

function getImageDimensions(file: File): Promise<{ width: number; height: number }> {
    return new Promise((resolve, reject) => {
        const url = URL.createObjectURL(file);

        const image = new Image();

        image.onload = () => {
            URL.revokeObjectURL(url);

            resolve({
                width: image.naturalWidth,
                height: image.naturalHeight
            });
        };

        image.onerror = () => {
            URL.revokeObjectURL(url);
            reject(new Error("Invalid image."));
        };

        image.src = url;
    });
}

async function validateImage(file: File): Promise<string | null> {
    const fileError = validateFile(file);

    if (fileError) return fileError;

    try {
        const { width, height } = await getImageDimensions(file);

        if (width < MIN_WIDTH || height < MIN_HEIGHT) {
            return `Image resolution is too small. Minimum size is ${MIN_WIDTH}x${MIN_HEIGHT}`;
        }

        if (width > MAX_WIDTH || height > MAX_HEIGHT) {
            return `Image resolution is too large. Maximum size is ${MAX_WIDTH}x${MAX_HEIGHT}`;
        }

        const aspectRatio = width / height;

        if (
            aspectRatio < MIN_ASPECT_RATIO ||
            aspectRatio > MAX_ASPECT_RATIO
        ) {
            return `Image aspect ratio must be between 1:${MAX_ASPECT_RATIO} and ${MAX_ASPECT_RATIO}:1.`;
        }

        return null;
    }
    catch {
        return "The selected file is not a valid image.";
    }
} 