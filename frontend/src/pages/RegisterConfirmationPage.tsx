import { ROUTES } from "../constants/routes";
import { Link, useLoaderData } from "react-router";
import RegisterCompletionComponent from "../features/auth/component/registerCompletion";
import { registerConfirmationLoader } from "../features/auth/loaders";

export default function RegisterConfirmationPage() {
    const state = useLoaderData<typeof registerConfirmationLoader>();

    switch (state.type) {
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