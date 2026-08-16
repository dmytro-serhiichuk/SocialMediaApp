import { useEffect, useRef } from "react";

export type InfiniteScrollStatus =
    | "idle"
    | "loading"
    | "error"
    | "finished";

export interface InfinityScrollProps {
    status: InfiniteScrollStatus,
    onLoadMore: () => void;
    loadingPlaceholer?: React.ReactNode;
    errorContent?: React.ReactNode;
    endContent?: React.ReactNode;
    rootMargin?: string
}

export function InfinityScroll({
    status, onLoadMore, loadingPlaceholer, 
    errorContent, endContent, rootMargin = "200px"
}: InfinityScrollProps) {
    const sentinelRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const sentinel = sentinelRef.current;

        if (!sentinel || status !== "idle") {
            return;
        }

        const observer = new IntersectionObserver(
            ([entry]) => {
                if (entry.isIntersecting) {
                    onLoadMore();
                }
            },
            {
                rootMargin: rootMargin,
            }
        );

        observer.observe(sentinel);

        return () => observer.disconnect();
    }, [status, onLoadMore]);

    switch (status) {
        case "loading": return loadingPlaceholer;
        case "error": return errorContent;
        case "finished": return endContent;
        default: return <div ref={sentinelRef} />;
    }
}