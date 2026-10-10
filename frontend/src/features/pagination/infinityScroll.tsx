import { useEffect, useRef } from "react";

export type InfinityScrollStatus =
    | "idle"
    | "loading"
    | "error"
    | "finished";

export interface InfinityScrollProps {
    status: InfinityScrollStatus,
    onLoadMore: () => void;
    loadingPlaceholer?: React.ReactNode;
    errorContent?: React.ReactNode;
    endContent?: React.ReactNode;
    rootMargin?: string;
    rootRef?: React.RefObject<HTMLElement | null>;
}

export function InfinityScroll({
    status, onLoadMore, loadingPlaceholer, 
    errorContent, endContent, rootMargin = "200px", rootRef
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
                root: rootRef?.current ?? null,
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