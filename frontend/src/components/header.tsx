import { Link } from "react-router";
import type { User } from "../features/auth/types";
import { ROUTES } from "../constants/routes";

export default function Header({user}: {user: User | null}) {
    return (
        <header>
            <Link to={ROUTES.HOME}>WebApp 1</Link>
            {user ? (
                <Link to={ROUTES.PROFILE}>Profile</Link>
            ) : (
                <>
                    <Link to={ROUTES.LOGIN}>Login</Link>
                    <Link to={ROUTES.REGISTER}>Create account</Link>
                </>
            )}
        </header>
    )
}