import { useRequiredUser } from "../features/auth/hooks";

export default function ProfilePage() {
    const user = useRequiredUser();

    return(
        <>
            <p>Id: {String(user.id)}</p>
            <p>Email: {user.Email}</p>
        </>
    );
}