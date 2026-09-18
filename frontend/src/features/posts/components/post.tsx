import { Link, useNavigate } from "react-router";
import type { Post } from "../types";
import { getPostEditPagePath, getTagPagePath, getUserPagePath } from "../../../utils/routerUtils";
import { useState } from "react";
import { apiClient } from "../../../api/client";
import { useUser } from "../../auth/hooks";
import ImageCarousel from "../../postImage/ImageCarousel";
import PostCard from "./postCard";
import PostExtended from "./postExtened";

export default function PostComponent({ post }: { post: Post }) {
    const navigate = useNavigate();
    const currentUser = useUser();
    const isOwnedByMe = post.user.id === currentUser?.id

    const [likesCount, setLikesCount] = useState<number>(post.likesCount);
    const [likedByMe, setLikedByMe] = useState<boolean>(post.isLikedByMe);
    const [savedByMe, setSavedByMe] = useState<boolean>(post.isSavedByMe);
    const [isBusy, setIsBusy] = useState<boolean>(false); 
    const [extendedCardVisible, setExtendedCardVisible] = useState(false);

    const handleLike = async () => {
        makeApiRequest(
            `posts/${post.id}/likes`, 
            likedByMe ? 'DELETE' : 'POST', 
            () => {
                setLikedByMe(prev => !prev);
                setLikesCount(prev => likedByMe ? prev - 1 : prev + 1);
        });
    }

    const handleSave = async () => {
        makeApiRequest(
            `posts/${post.id}/save`, 
            savedByMe ? 'DELETE' : 'POST', 
            () => setSavedByMe(prev => !prev));
    }

    const makeApiRequest = async (endpoint: string, method: string, func: () => void) => {
        try {
            setIsBusy(true);

            await apiClient(endpoint, {
                method: method
            });

            func();
        } catch (error) {
            if (error instanceof Error) {
                alert(error.message);
            } else {
                alert('Something went wrong');
            }
        } finally {
            setIsBusy(false);
        }
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