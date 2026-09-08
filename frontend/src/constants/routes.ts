export const ROOT_ID = 'root';

export const ROUTES = Object.freeze({
    HOME: "/",
    USER: "/users/:userTag",
    LOGIN: "/login",
    REGISTER: "/register",
    REGISTER_CONFIRM: "/register/confirm",
    LOGOUT: "/logout",
    POST_NEW: "/posts/new",
    TAG: "/tags/:tagName",
    SAVED_POSTS: "/posts/saved",
    SETTINGS: "/settings",
    EMAIL_CHANGE: "/settings/email-change/confirm",
    PASSWORD_RESET_REQUEST: "/password-reset",
    PASSWORD_RESET_COMPLETE: "/password-reset/complete",
})