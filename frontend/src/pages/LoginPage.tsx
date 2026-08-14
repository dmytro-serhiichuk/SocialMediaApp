import { Form, useActionData } from "react-router";
import type { loginAction } from "../features/auth/actions";

export default function LoginPage() {
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
        </>
    );
}