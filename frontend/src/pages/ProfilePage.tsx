import { useRequiredUser } from "../features/auth/hooks";

export default function ProfilePage() {
    const user = useRequiredUser();

    return(
        <>
            <p>UserName: {user.userName}</p>
            <p>Tag: {user.userTag}</p>
            <p>Email: {user.email}</p>
        </>
    );
}