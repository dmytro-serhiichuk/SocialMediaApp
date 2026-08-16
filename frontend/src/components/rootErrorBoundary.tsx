import { isRouteErrorResponse, useRouteError } from "react-router";
import NotFound from "../pages/NotFound";
import { ApiError } from "../api/client";

export default function RootErrorBoundary() {
    const error = useRouteError();

    if ((isRouteErrorResponse(error) && error.status === 404) || 
        (error instanceof ApiError && error.status === 404)) {
        return <NotFound />
    }

    return (
        <div>
            <h1>Something went wrong...</h1>
            <p>{error instanceof Error ? error.message : "Unknown error"}</p>
        </div>
    )
}