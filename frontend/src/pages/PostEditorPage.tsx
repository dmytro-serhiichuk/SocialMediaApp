import { Form, useActionData } from "react-router";
import type { createPostAction } from "../features/posts/actions";
import { useState } from "react";
import { TagInput } from "../features/tag/components/tagInput";
import ValidationErrorsList from "../components/validationErrorsList";
import ImageInput from "../features/postImage/imageInput";

export default function PostEditorPage() {
    const actionData = useActionData<typeof createPostAction>();

    const [tags, setTags] = useState<string[]>([]);

    return (
        <>
            <h1>Post Editor</h1>

            <Form method="post" encType="multipart/form-data">
                <ImageInput />

                <textarea name="content" placeholder="Enter your text..."/>

                {actionData?.error && (
                    <p>{actionData.error}</p>
                )}
                {actionData?.validationErrors && <ValidationErrorsList errors={actionData?.validationErrors} />}

                {tags.map(tag => (
                    <input key={tag} type="hidden" name="tags" value={tag}/>
                ))}

                <TagInput tags={tags} onChange={setTags}/>

                <button type="submit">Publish</button>
            </Form>
        </>
    );
}