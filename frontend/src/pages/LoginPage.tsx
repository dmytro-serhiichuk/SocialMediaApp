import { Form, Link, useActionData, useLoaderData } from "react-router";
import type { loginAction } from "../features/auth/actions";
import { ROUTES } from "../constants/routes";
import LoginViaGoogleButton from "../features/auth/component/loginViaGoogleButton";
import { oauthLoader } from "../features/auth/loaders";

export default function LoginPage() {
    const oauthError = useLoaderData<typeof oauthLoader>();
    const actionData = useActionData<typeof loginAction>();

    return (
        <>
            <p>Login page</p>

            {/* TODO: add password double check and data validation */}
            <Form method="post">
                <label>
                    Login
                    <input name="login"/>
                </label>
                <label>
                    Password
                    <input name="password" type="password"/>
                </label>

                {actionData?.error && (
                    <p>{actionData.error}</p>
                )}

                <button type="submit">Login</button>
            </Form>

            {oauthError && <p>{oauthError}</p>}

            <LoginViaGoogleButton />

            <Link to={ROUTES.PASSWORD_RESET_REQUEST}>Forgot password</Link>
        </>
    );
}