import { Navigate, Outlet } from "react-router";
import { useUser } from "../features/auth/hooks";
import { getUserPagePath } from "../utils/routerUtils";

export default function GuestOnlyLayout() {
    const user = useUser();

    if (user) {
        return <Navigate to={getUserPagePath(user.userTag)} replace />;
    }

    return <Outlet />;
}