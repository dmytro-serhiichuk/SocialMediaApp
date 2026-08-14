import { Form, useActionData } from "react-router";
import type { registerAction } from "../features/auth/actions";

export default function RegisterPage() {
    const actionData = useActionData<typeof registerAction>();
    
    return (
        <>
            <p>Register page</p>

            {/* TODO: add password double check and data validation */}
            <Form method="post">
                <label>
                    User Name
                    <input name="userName"/>
                </label>
                <label>
                    User Tag
                    <input name="userTag"/>
                </label>
                <label>
                    Email
                    <input name="email" type="email"/>
                </label>
                <label>
                    Password
                    <input name="password" type="password"/>
                </label>

                {actionData?.error && (
                    <p>{actionData.error}</p>
                )}

                <button type="submit">Register</button>
            </Form>
        </>
    );
}