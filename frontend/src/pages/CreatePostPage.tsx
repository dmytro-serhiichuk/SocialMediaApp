import { useActionData } from "react-router";
import type { createPostAction } from "../features/posts/actions";
import PostEditor from "../features/posts/components/postEditor";

export default function CreatePostPage() {
    const actionData = useActionData<typeof createPostAction>();

    return (
        <>
            <h1>New Post</h1>

            <PostEditor
                error={actionData?.error}
                validationErrors={actionData?.validationErrors}
                submitLabel="Publish" />
        </>
    );
}