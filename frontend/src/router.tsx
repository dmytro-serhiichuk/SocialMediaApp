import { createBrowserRouter } from "react-router";
import RootLayout from "./layouts/RootLayout";
import HomePage from "./pages/HomePage";
import NotFound from "./pages/NotFound";
import ProtectedLayout from "./layouts/ProtectedLayout";
import { accountRecoveryLoader, authLoader } from "./features/auth/loaders";
import UserPage from "./pages/UserPage";
import { ROOT_ID, ROUTES } from "./constants/routes";
import GuestOnlyLayout from "./layouts/GuestOnlyLayout";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import { completePasswordResetAction, completeRegisterAction, loginAction, logoutAction, recoverAccountAction, registerAction, requestPasswordResetAction } from "./features/auth/actions";
import RootErrorBoundary from "./components/rootErrorBoundary";
import PostEditorPage from "./pages/PostEditorPage";
import { createPostAction } from "./features/posts/actions";
import { userLoader } from "./features/user/loaders";
import TagPage from "./pages/TagPage";
import { tagLoader } from "./features/tag/loaders";
import SavedPostsPage from "./pages/SavedPostsPage";
import SettingsPage from "./pages/SettingsPage";
import { settingsLoader } from "./features/settings/loaders";
import { settingsAction } from "./features/settings/actions";
import EmailChangeConfirmationPage from "./pages/EmailChangeConfirmationPage";
import RegisterConfirmationPage from "./pages/RegisterConfirmationPage";
import PasswordResetRequestPage from "./pages/PasswordResetRequestPage";
import CompletePasswordResetPage from "./pages/CompletePasswordResetPage";
import AccountRecoveryPage from "./pages/AccountRecoveryPage";

const router = createBrowserRouter([
    {
        id: ROOT_ID,
        path: ROUTES.HOME,
        Component: RootLayout,
        loader: authLoader,
        shouldRevalidate: () => false,
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
                path: ROUTES.EMAIL_CHANGE,
                Component: EmailChangeConfirmationPage
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
                    },
                    {
                        path: ROUTES.SAVED_POSTS,
                        Component: SavedPostsPage
                    },
                    {
                        path: ROUTES.SETTINGS,
                        Component: SettingsPage,
                        loader: settingsLoader,
                        action: settingsAction
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
                    {
                        path: ROUTES.REGISTER_CONFIRM,
                        Component: RegisterConfirmationPage,
                        action: completeRegisterAction
                    },
                    {
                        path: ROUTES.PASSWORD_RESET_REQUEST,
                        Component: PasswordResetRequestPage,
                        action: requestPasswordResetAction
                    },
                    {
                        path: ROUTES.PASSWORD_RESET_COMPLETE,
                        Component: CompletePasswordResetPage,
                        action: completePasswordResetAction
                    },
                    {
                        path: ROUTES.ACCOUNT_RECOVERY,
                        Component: AccountRecoveryPage,
                        loader: accountRecoveryLoader,
                        action: recoverAccountAction
                    }
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