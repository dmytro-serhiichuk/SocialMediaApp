export function getExternalLoginError(): string | null {
    const url = new URL(window.location.href);
    const error = url.searchParams.get("error");
    const provider = url.searchParams.get("provider");

    window.history.replaceState(null, "", url.pathname);

    if (error === "oauth_failed" || error === "connect_failed" || error === "login_failed") {
        return "Authentication failed. Please try again";
    } else if (error === "email_taken") {
        return `An account with this email already exists. Sign in to your existing account and connect ${provider} from Settings`;
    } else if (error === "already_linked") {
        return `This ${provider} account is already connected to another profile`;
    } else if (error === "unknown" || error === "unexpected") {
        return "Unexpected error occurred. Pleasy try again";
    }

    return null
}