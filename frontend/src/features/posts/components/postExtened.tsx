import { Link } from "react-router";
import { getTagPagePath, getUserPagePath } from "../../../utils/routerUtils";
import ImageCarousel from "../../postImage/ImageCarousel";
import type { Post } from "../types";
import CommentList from "../../comments/components/commentsList";
import { apiClient } from "../../../api/client";
import { useState } from "react";
import { useUser } from "../../auth/hooks";

export default function PostExtended({ post }: { post: Post }) {
    const currentUser = useUser();
    const [likesCount, setLikesCount] = useState<number>(post.likesCount);
    const [likedByMe, setLikedByMe] = useState<boolean>(post.isLikedByMe);
    const [savedByMe, setSavedByMe] = useState<boolean>(post.isSavedByMe);
    const [isBusy, setIsBusy] = useState<boolean>(false); 

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

    return (
        <div className="post-extended-container">
            <div className="post-extended-image">
                <ImageCarousel 
                    images={post.images.map(img => ({
                        type: "existing",
                        id: img.id,
                        url: img.url
                    }))}/>
            </div>

            <div className="post-extended-body">
                <Link to={getUserPagePath(post.user.userTag)}>
                    <p>{post.user.userName}</p>
                    <p>{post.user.userTag}</p>
                </Link>

                <p>{new Date(post.createdAt).toISOString()}</p>
                {post.updatedAt && (<p>*Updated {new Date(post.updatedAt).toISOString()}</p>)}

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
                        <button onClick={handleLike} disabled={isBusy}>{likedByMe ? "Unlike" : "Like"}</button>

                        <button onClick={handleSave} disabled={isBusy}>{savedByMe ? "Unsave" : "Save"}</button>
                    </>
                }
            
                <CommentList />
            </div>

        </div>
    );
}