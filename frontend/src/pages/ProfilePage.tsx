import { useRequiredUser } from "../features/auth/hooks";

export default function ProfilePage() {
    const user = useRequiredUser();

    return(
        <>
            <p>UserName: {user.UserName}</p>
            <p>Email: {user.Email}</p>
        </>
    );
}