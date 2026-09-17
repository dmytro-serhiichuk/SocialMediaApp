import { Form, useSubmit } from "react-router";
import ImageInput from "../../postImage/imageInput";
import type { ValidationErrors } from "../../../api/client";
import ValidationErrorsList from "../../../components/validationErrorsList";
import { useState } from "react";
import { TagInput } from "../../tag/components/tagInput";
import type { Image } from "../../image/types";
import type { InputImageInfo, InputImageInfoRequest } from "../../postImage/types";

type PostEditorProps = {
    content?: string;
    existingImages?: Image[];
    tags?: string[];
    error?: string;
    validationErrors?: ValidationErrors;
    submitLabel: string;
};

export default function PostEditor({
    content = "",
    existingImages = [],
    tags: initialTags = [],
    error,
    validationErrors,
    submitLabel
}: PostEditorProps) {
    const submit = useSubmit();
    const [images, setImages] = useState<InputImageInfo[]>(existingImages.map(i => ({
        type: "existing",
        id: i.id,
        url: i.url
    })));
    const [tags, setTags] = useState(initialTags);

    // TODO: add validation and check if data was actually changed
    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const formData = new FormData();

        formData.append("content", e.currentTarget.content.value)

        tags.forEach(tag => formData.append("tags", tag));

        const infos: InputImageInfoRequest[] = [];
        let index = 0;

        images.forEach(image => {
            if (image.type === "existing") {
                infos.push({
                    imageId: image.id
                });
            } else {
                infos.push({
                    index: index++
                });
                formData.append("images", image.file);
            }
        });

        formData.append("info", JSON.stringify(infos));

        submit(formData, {
            method: "post",
            encType: "multipart/form-data"
        });
    }

    return (
        <Form method="post" encType="multipart/form-data" onSubmit={handleSubmit}>
            <ImageInput images={images} onChange={setImages} />

            <textarea 
                name="content" 
                placeholder="Enter your text..." 
                defaultValue={content}/>

            {error && (
                <p>{error}</p>
            )}
            {validationErrors && (
                <ValidationErrorsList errors={validationErrors} />
            )}

            {tags.map(tag => (
                <input key={tag} type="hidden" name="tags" value={tag}/>
            ))}

            <TagInput tags={tags} onChange={setTags}/>

            <button type="submit">
                {submitLabel}
            </button>
        </Form>
    );
}