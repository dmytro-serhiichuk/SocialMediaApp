import { Outlet } from "react-router";
import Header from "../features/header/header";
import { useUser } from "../features/auth/hooks";

export default function RootLayout() {
    const user = useUser();

    return (
        <>
            <Header user={user}/>

            <main>
                <Outlet/>
            </main>

            <footer>footer</footer>
        </>
    );
}