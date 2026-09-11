import { useState } from "react";
import { Form, Link, useActionData, useLoaderData, useNavigation } from "react-router";
import ValidationErrorsList from "../components/validationErrorsList";
import { completePasswordResetLoader } from "../features/auth/loaders";
import { ROUTES } from "../constants/routes";
import type { completePasswordCreationAction } from "../features/settings/actions";

export default function CompletePasswordCreationPage() {
    const token = useLoaderData<typeof completePasswordResetLoader>();
    const actionData = useActionData<typeof completePasswordCreationAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";

    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const valid = newPassword.length > 0 && newPassword === confirmPassword;

    if (!token) {
        return (
            <>
                <p>Password creation link is invalid or expired.</p>
                <p>You can request a new password creation link in settings.</p>
                <Link to={ROUTES.SETTINGS}>Settings</Link>
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
                    {isSubmitting ? "Submitting..." : "Create password"}
                </button>
            </Form>

        </>
    );
}