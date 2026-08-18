import { Form, Link } from "react-router";
import type { User } from "../auth/types";
import { ROUTES } from "../../constants/routes";
import { getUserPagePath } from "../../utils/routerUtils";
import SearchBar from "./searchBar";

export default function Header({user}: {user: User | null}) {
    return (
        <header>
            <Link to={ROUTES.HOME}>WebApp 1</Link>
            {user ? (
                <>
                    <Link to={getUserPagePath(user.userTag)}>Profile</Link>
                    <Link to={ROUTES.POST_NEW}>Create New Post</Link>
                    <Form method="post" action={ROUTES.LOGOUT}>
                        <button type="submit">
                            Logout
                        </button>
                    </Form>
                </>
            ) : (
                <>
                    <Link to={ROUTES.LOGIN}>Login</Link>
                    <Link to={ROUTES.REGISTER}>Create account</Link>
                </>
            )}

            <SearchBar />
        </header>
    )
}