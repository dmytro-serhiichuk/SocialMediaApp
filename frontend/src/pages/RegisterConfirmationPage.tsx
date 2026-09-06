import { useEffect, useState } from "react";
import { ApiError } from "../api/client";
import { confirmRegister } from "../features/auth/api";
import { ROUTES } from "../constants/routes";
import { Link } from "react-router";
import RegisterCompletionComponent from "../features/auth/component/registerCompletion";

type ConfirmationState =
    | { type: "loading" }
    | { type: "success" }
    | { type: "invalid-token" }
    | { type: "email-conflict" }
    | { type: "tag-conflict"; token: string };

export default function RegisterConfirmationPage() {
    const [state, setState] = useState<ConfirmationState>({ type: "loading" });
    const [unexpectedError, setUnexpectedError] = useState<unknown>(null);

    useEffect(() => {
        const params = new URLSearchParams(window.location.hash.slice(1));
        const token = params.get("token");
        window.history.replaceState(null, "", window.location.pathname);

        if (!token) {
            setState({ type: "invalid-token" });
            return;
        }

        (async () => {
            try {
                await confirmRegister(token);

                setState({ type: "success" });
            } catch (error) {
                if (error instanceof ApiError) {
                    if (error.status === 409 && error.validationErrors) {
                        if ("email" in error.validationErrors) {
                            setState({ type: "email-conflict" });
                        } else if ("tag" in error.validationErrors) {
                            setState({ type: "tag-conflict", token: token });
                        }
                    } else if (error.status === 400) {
                        setState({ type: "invalid-token" });
                    } else {
                        setUnexpectedError(error);
                    }
                } else {
                    setUnexpectedError(error);
                }
            }
        })();
    }, []);
    
    if (unexpectedError) {
        throw unexpectedError;
    }

    switch (state.type) {
        case "loading":
            return <p>Processing request...</p>;
        
        case "success":
            return <p>Your request is confirmed</p>;
        
        case "email-conflict":
            return (
                <>
                    <p>User with this email already exists</p>
                    <Link to={ROUTES.LOGIN}>Login</Link>
                </> 
            );
        
        case "invalid-token":
            return <p>Confirmation request is invalid</p>
        
        case "tag-conflict":
            return <RegisterCompletionComponent token={state.token} />
    }
}