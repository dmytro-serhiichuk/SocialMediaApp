import { useEffect, useRef, useState } from "react";
import { Form, useActionData, useNavigation } from "react-router";
import type { completePasswordResetAction } from "../features/auth/actions";
import ValidationErrorsList from "../components/validationErrorsList";

type TokenState = "ok" | "error" | "loading";

export default function CompletePasswordResetPage() {
    const actionData = useActionData<typeof completePasswordResetAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";

    const initializedRef = useRef(false);
    const [token, setToken] = useState<string>("");
    const [tokenState, setTokenState] = useState<TokenState>("loading");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    useEffect(() => {
        if (initializedRef.current) return;

        initializedRef.current = true;

        const params = new URLSearchParams(window.location.hash.slice(1));
        const tokenParam = params.get("token");
        window.history.replaceState(null, "", window.location.pathname);

        if (!tokenParam) {
            setTokenState("error");
        } else {
            setTokenState("ok");
            setToken(tokenParam);
        }
    }, []);

    const valid = newPassword.length > 0 && newPassword === confirmPassword;

    switch (tokenState) {
        case "loading":
            return <p>Loading...</p>;
        
        case "error":
            return <p>Invalid request</p>;

        case "ok":
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
}