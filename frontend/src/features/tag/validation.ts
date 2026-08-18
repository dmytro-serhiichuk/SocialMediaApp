const regex = /^[a-z0-9_]+$/i;
const minLength = 3;
const maxLength = 32;

export function normalizeTagName(tagName: string) {
    return tagName.trim().toLocaleLowerCase();
}

export function isTagNameValid(tagName: string) {
    return tagName.length >= minLength && tagName.length <= maxLength && regex.test(tagName);
}