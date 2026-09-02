import { useEffect, useState } from "react";
import { ApiError } from "../api/client";
import { confirmEmailChange } from "../features/settings/api";

type Status = "loading" | "success" | "error";

export default function EmailChangeConfirmationPage() {
    const [status, setStatus] = useState<Status>("loading");
    const [error, setError] = useState<string>();
    const [unexpectedError, setUnexpectedError] = useState<unknown>(null);

    useEffect(() => {
        const params = new URLSearchParams(window.location.hash.slice(1));
        const token = params.get("token");
        window.history.replaceState(null, "", window.location.pathname);

        if (!token) {
            setStatus("error");
            setError("Confirmation request is invalid");
            return;
        } 

        (async () => {
            try {
                await confirmEmailChange(token);

                setStatus("success");
            } catch (error) {
                setStatus("error");
                if (error instanceof ApiError && (error.status === 400 || error.status === 409)) {
                    setError(error.message);
                } else {
                    setUnexpectedError(error);
                }
            }
        })();
    }, []);
    
    if (unexpectedError) {
        throw unexpectedError;
    }

    if (status === "loading") {
        return <p>Processing request...</p>;
    }

    if (status === "success") {
        return <p>Your request is confirmed</p>;
    }

    return <p>Error: {error}</p>;
}