import { Link, useNavigate } from "react-router";
import type { Post } from "../types";
import { getPostEditPagePath, getTagPagePath, getUserPagePath } from "../../../utils/routerUtils";
import { useState } from "react";
import { useUser } from "../../auth/hooks";
import ImageCarousel from "../../postImage/ImageCarousel";
import PostCard from "./postCard";
import PostExtended from "./postExtended";
import { useToggleApiRequest } from "../../../api/hooks";

export default function PostComponent({ post }: { post: Post }) {
    const navigate = useNavigate();
    const currentUser = useUser();
    const isOwnedByMe = post.user.id === currentUser?.id

    const [likesCount, setLikesCount] = useState<number>(post.likesCount);
    const [likedByMe, setLikedByMe] = useState<boolean>(post.isLikedByMe);
    const [savedByMe, setSavedByMe] = useState<boolean>(post.isSavedByMe);
    const [isBusy, setIsBusy] = useState<boolean>(false);
    const { toggleApiRequest } = useToggleApiRequest(setIsBusy);
    const [extendedCardVisible, setExtendedCardVisible] = useState(false);

    const handleLike = async () => {
        toggleApiRequest(
            `posts/${post.id}/likes`, 
            likedByMe ? 'DELETE' : 'POST', 
            () => {
                setLikedByMe(prev => !prev);
                setLikesCount(prev => likedByMe ? prev - 1 : prev + 1);
        });
    }

    const handleSave = async () => {
        toggleApiRequest(
            `posts/${post.id}/save`, 
            savedByMe ? 'DELETE' : 'POST', 
            () => setSavedByMe(prev => !prev));
    }

    const handleCommentsToggle = () => {
        setExtendedCardVisible(!extendedCardVisible);
    }

    return (
        <div className="post-container">
            {isOwnedByMe && (
                <button onClick={() => navigate(getPostEditPagePath(post.id))}>
                    Edit
                </button>
            )}

            <ImageCarousel 
                images={post.images.map(img => ({
                    type: "existing",
                    id: img.id,
                    url: img.url
                }))}/>

            <Link to={getUserPagePath(post.user.userTag)}>
                <p>{post.user.userName}</p>
                <p>{post.user.userTag}</p>
            </Link>

            <p>{new Date(post.createdAt).toISOString()}</p>
            {post.updatedAt && (
                <p><i>Edited</i> {new Date(post.updatedAt).toISOString()}</p>
            )}

            <div>
                <p>{post.content}</p>
            </div>

            {post.tags.map(t => (
                <div className="tag-container" key={t.id}>
                    <Link to={getTagPagePath(t.name)}>{t.name}</Link>
                </div>
            ))}

            <p>Likes: {likesCount}</p>
            {currentUser && 
                <>
                    <button onClick={handleLike} disabled={isBusy}>
                        {likedByMe ? "Unlike" : "Like"}
                    </button>

                    <button onClick={handleSave} disabled={isBusy}>
                        {savedByMe ? "Unsave" : "Save"}
                    </button>
                </>
            }

            <button onClick={handleCommentsToggle}>
                Show comments
            </button>

            {extendedCardVisible && (
                <PostCard onClose={() => setExtendedCardVisible(false)}>
                    <PostExtended post={post} />
                </PostCard>
            )}
        </div>
    );
}