import { createBrowserRouter } from "react-router";
import RootLayout from "./layouts/RootLayout";
import HomePage from "./pages/HomePage";
import NotFound from "./pages/NotFound";
import ProtectedLayout from "./layouts/ProtectedLayout";
import { authLoader } from "./features/auth/loaders";
import ProfilePage from "./pages/ProfilePage";
import { ROOT_ID, ROUTES } from "./constants/routes";
import GuestOnlyLayout from "./layouts/GuestOnlyLayout";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import { loginAction, logoutAction, registerAction } from "./features/auth/actions";

const router = createBrowserRouter([
    {
        id: ROOT_ID,
        path: ROUTES.HOME,
        Component: RootLayout,
        loader: authLoader,
        children: [
            {
                index: true,
                Component: HomePage
            },
            {
                Component: ProtectedLayout,
                children: [
                    {
                        path: ROUTES.PROFILE,
                        Component: ProfilePage
                    },
                    {
                        path: ROUTES.LOGOUT,
                        action: logoutAction
                    }
                ]
            },
            {
                Component: GuestOnlyLayout,
                children: [
                    {
                        path: ROUTES.LOGIN,
                        Component: LoginPage,
                        action: loginAction
                    },
                    {
                        path: ROUTES.REGISTER,
                        Component: RegisterPage,
                        action: registerAction
                    },
                ]
            },
            {
                path: "*",
                Component: NotFound
            }
        ]
    }
]);

export default router;