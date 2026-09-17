import { useEffect, useState } from "react";
import type { InputImageInfo } from "./types";
import ImagePreview from "./imagePreview";

type ImageCarouselProps = {
    images: InputImageInfo[];
    onRemove?: (index: number) => void;
};

export default function ImageCarousel({images, onRemove}: ImageCarouselProps) {
    const [currentIndex, setCurrentIndex] = useState(0);
    const currentImage = images[currentIndex];

    useEffect(() => {
        if (currentIndex >= images.length) {
            setCurrentIndex(Math.max(0, images.length - 1));
        }
    }, [images.length, currentIndex]);

    const moveLeft = () => {
        setCurrentIndex((currentIndex - 1 + images.length) % images.length)
    }
    const moveRight = () => {
        setCurrentIndex((currentIndex + 1) % images.length)
    }

    const handleRemove = () => {
        const index = currentIndex;
        setCurrentIndex(Math.max(0, images.length - 2));
        if (onRemove) onRemove(index);
    }

    if (images.length === 0) return null;
    
    return (
        <div className="image-viewer">
            <ImagePreview image={currentImage}/>
            {onRemove && (
                <button 
                    type="button" 
                    onClick={handleRemove}>
                        Remove
                </button>
            )}

            {images.length > 1 && (
                <>
                    <button
                        type="button"
                        onClick={moveLeft}>
                        ←
                    </button>

                    <button
                        type="button"
                        onClick={moveRight}>
                        →
                    </button>

                    <span>
                        {currentIndex + 1} / {images.length}
                    </span>
                </>
            )}
        </div>
    );
}