import { Form, useActionData } from "react-router";
import type { createPostAction } from "../features/posts/actions";

export default function PostEditorPage() {
    const actionData = useActionData<typeof createPostAction>();

    return (
        <>
            <h1>Post Editor</h1>

            <Form method="post">
                <textarea name="content" placeholder="Enter your text..."/>

                {actionData?.error && (
                    <p>{actionData.error}</p>
                )}

                <button type="submit">Publish</button>
            </Form>
        </>
    );
}