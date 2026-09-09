import { Form, Link, useActionData, useLoaderData, useNavigation } from "react-router";
import { accountRecoveryLoader } from "../features/auth/loaders";
import { ROUTES } from "../constants/routes";
import { useState } from "react";
import type { recoverAccountAction } from "../features/auth/actions";
import ValidationErrorsList from "../components/validationErrorsList";

export default function AccountRecoveryPage() {
    const result = useLoaderData<typeof accountRecoveryLoader>();
    const actionData = useActionData<typeof recoverAccountAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";

    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const valid = newPassword.length > 0 && newPassword === confirmPassword;

    if (!result.token) {
        return <p>Recovery link is invalid</p>
    }
    else if (result.recoveryNotAvailable || actionData?.recoveryNotAvailable) {
        return <p>Access to this account has been blocked and cannot be restored in this way</p>
    }
    else if (result.tokenIsNotValid) {
        return (
            <>
                <p>This recovery link is no longer valid. You can reset your password to regain access</p>
                <Link to={ROUTES.PASSWORD_RESET_REQUEST}>Reset password</Link>
            </>
        )
    }
    else {
         return (
            <>
                <p>Recover your account</p>

                <Form method="post">
                    <input type="hidden" name="token" value={result.token} />
                                            
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
}