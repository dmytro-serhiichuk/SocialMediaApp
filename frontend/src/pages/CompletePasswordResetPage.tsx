import { useState } from "react";
import { Form, Link, useActionData, useLoaderData, useNavigation } from "react-router";
import type { completePasswordResetAction } from "../features/auth/actions";
import ValidationErrorsList from "../components/validationErrorsList";
import { completePasswordResetLoader } from "../features/auth/loaders";
import { ROUTES } from "../constants/routes";

export default function CompletePasswordResetPage() {
    const token = useLoaderData<typeof completePasswordResetLoader>();
    const actionData = useActionData<typeof completePasswordResetAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";

    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const valid = newPassword.length > 0 && newPassword === confirmPassword;

    if (!token) {
        return (
            <>
                <p>Password reset link is invalid or expired. Your password was not changed.</p>
                <p>You can request a new password reset link.</p>
                <Link to={ROUTES.PASSWORD_RESET_REQUEST}>Reset Password</Link>
            </>
        )
    }

    return (
        <>
            <p>Enter your new password</p>

            <Form method="post">
                <input type="hidden" name="token" value={token} />
                
                {/* TODO: add "eye" button */}
                <label>
                    New password
                    <input name="new-password" type="password" value={newPassword} onChange={e => setNewPassword(e.target.value)}/>
                </label>
                <label>
                    Confirm new password
                    <input type="password" value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)}/>
                </label>

                {actionData?.error && (
                    <p>{actionData.error}</p>
                )}
                {actionData?.validationErrors && (
                    <ValidationErrorsList errors={actionData?.validationErrors} />
                )}

                <button type="submit" disabled={isSubmitting || !valid}>
                    {isSubmitting ? "Submitting..." : "Reset password"}
                </button>
            </Form>

        </>
    );
}