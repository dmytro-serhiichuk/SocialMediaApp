import { useRouteError } from "react-router";

export default function RootErrorBoundary() {
    const error = useRouteError();

    return (
        <div>
            <h1>Something went wrong...</h1>
            <p>{error instanceof Error ? error.message : "Unknown error"}</p>
        </div>
    )
}