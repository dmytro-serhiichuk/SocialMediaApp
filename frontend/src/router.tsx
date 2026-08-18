import { createBrowserRouter } from "react-router";
import RootLayout from "./layouts/RootLayout";
import HomePage from "./pages/HomePage";
import NotFound from "./pages/NotFound";
import ProtectedLayout from "./layouts/ProtectedLayout";
import { authLoader } from "./features/auth/loaders";
import UserPage from "./pages/UserPage";
import { ROOT_ID, ROUTES } from "./constants/routes";
import GuestOnlyLayout from "./layouts/GuestOnlyLayout";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import { loginAction, logoutAction, registerAction } from "./features/auth/actions";
import RootErrorBoundary from "./components/rootErrorBoundary";
import PostEditorPage from "./pages/PostEditorPage";
import { createPostAction } from "./features/posts/actions";
import { userLoader } from "./features/user/loaders";
import TagPage from "./pages/TagPage";
import { tagLoader } from "./features/tag/loaders";

const router = createBrowserRouter([
    {
        id: ROOT_ID,
        path: ROUTES.HOME,
        Component: RootLayout,
        loader: authLoader,
        ErrorBoundary: RootErrorBoundary,
        children: [
            {
                index: true,
                Component: HomePage
            },
            {
                path: ROUTES.USER,
                Component: UserPage,
                loader: userLoader
            },
            {
                path: ROUTES.TAG,
                Component: TagPage,
                loader: tagLoader
            },
            {
                Component: ProtectedLayout,
                children: [
                    {
                        path: ROUTES.LOGOUT,
                        action: logoutAction
                    },
                    {
                        path: ROUTES.POST_NEW,
                        Component: PostEditorPage,
                        action: createPostAction
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