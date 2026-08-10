import { Navigate, Outlet } from "react-router";
import { ROUTES } from "../constants/routes";
import { useUser } from "../features/auth/hooks";

export default function GuestOnlyLayout() {
    const user = useUser();

    if (user) {
        return <Navigate to={ROUTES.PROFILE} replace />;
    }

    return <Outlet />;
}