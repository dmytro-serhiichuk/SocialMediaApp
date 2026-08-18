import { Outlet, useLoaderData } from "react-router";
import Header from "../features/header/header";
import type { User } from "../features/auth/types";

export default function RootLayout() {
    const user = useLoaderData<User | null>();

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