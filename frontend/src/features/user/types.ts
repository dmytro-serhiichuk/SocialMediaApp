export interface UserProfile {
    id: number,
    userName: string,
    userTag: string,
    followingCount: number,
    followersCount: number,
    isFollowedByMe: boolean,
    isFollowingMe: boolean
}