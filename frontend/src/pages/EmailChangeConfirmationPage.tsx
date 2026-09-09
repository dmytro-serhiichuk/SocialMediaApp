import { Link, useLoaderData } from "react-router";
import { emailChangeConfirmLoader } from "../features/settings/loaders";
import { ROUTES } from "../constants/routes";

export default function EmailChangeConfirmationPage() {
    const isConfirmed = useLoaderData<typeof emailChangeConfirmLoader>();

    if (!isConfirmed) {
        return (
            <>
                <p>Confirmation link is invalid or expired. You can try to change your email in settings again</p>
                <Link to={ROUTES.SETTINGS}>Go to Settings</Link>
            </>
        );
    }

    return <p>Your request is confirmed</p>;
}