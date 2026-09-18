import { useEffect } from "react";

interface PostCardProps extends React.PropsWithChildren {
    onClose: () => void
}

export default function PostCard({ onClose, children }: PostCardProps) {
    useEffect(() => {
        document.body.classList.add("no-scroll");

        return () => {
            document.body.classList.remove("no-scroll");
        };
    }, []);

    return (
        <div className="post-card">
            <button 
                className="post-card-close-button"
                onClick={onClose}>
                Close
            </button>
            <div className="post-card-body">
                {children}
            </div>
        </div>
    );
}