export function getUserPagePath(userTag: string) {
    return `/users/${userTag}`;
}

export function getUserFollowersPagePath(userTag: string) {
    return `/users/${userTag}/followers`;
}

export function getUserFollowingPagePath(userTag: string) {
    return `/users/${userTag}/following`;
}

export function getTagPagePath(tagName: string) {
    return `/tags/${tagName}`;
}