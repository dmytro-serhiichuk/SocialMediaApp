const regex = /^[a-z0-9_]+$/i;
const minLength = 3;
const maxLength = 50;

export function normalizeUserTag(userTag: string) {
    return userTag.trim().toLocaleLowerCase();
}

export function isUserTagValid(userTag: string) {
    return userTag.length >= minLength && userTag.length <= maxLength && regex.test(userTag);
}