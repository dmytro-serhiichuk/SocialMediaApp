import { useActionData, useLoaderData } from "react-router";
import type { updatePostAction } from "../features/posts/actions";
import PostEditor from "../features/posts/components/postEditor";
import { editPostLoader } from "../features/posts/loaders";

export default function EditPostPage() {
    const post = useLoaderData<typeof editPostLoader>();
    const actionData = useActionData<typeof updatePostAction>();

    return (
        <>
            <h1>Edit Post</h1>

            <PostEditor
                content={post.content}
                existingImages={post.images}
                tags={post.tags}
                error={actionData?.error}
                validationErrors={actionData?.validationErrors}
                submitLabel="Save Changes" />
        </>
    );
}