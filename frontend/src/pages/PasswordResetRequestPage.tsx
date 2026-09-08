import { Form, useActionData, useNavigation } from "react-router";
import type { requestPasswordResetAction } from "../features/auth/actions";

export default function PasswordResetRequestPage() {
    const actionData = useActionData<typeof requestPasswordResetAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";

    return (
        <>
            <h3>Reset your password</h3>

            {actionData?.success ? 
                <p>Check your email to follow a password reset link</p>
                :
                <>
                    <p>Enter your user account's email to receive a password reset link</p>

                    <Form method="post">
                        <label>
                            Email
                            <input name="email" type="email"/>
                        </label>

                        {actionData?.error && (
                            <p>{actionData.error}</p>
                        )}

                        <button
                            type="submit"
                            disabled={isSubmitting}>
                            Submit
                        </button>
                    </Form>
                </>
            }
        </>
    );
}