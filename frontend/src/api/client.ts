import { API_URL } from "../constants";
import { authStore } from "../features/auth/store";

export type ValidationErrors = Record<string, string[]>;

type ProblemDetails = {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    errors?: ValidationErrors;
};

export class ApiError extends Error {
    public status: number;
    public problem?: ProblemDetails;
    
    constructor(
        status: number,
        problem?: ProblemDetails
    ) {
        super(problem?.title ?? `API request failed: ${status}`);
        this.status = status;
        this.problem = problem;
    }

    get validationErrors() {
        return this.problem?.errors;
    }

    isValidationError() {
        return this.problem?.errors !== undefined;
    }
}

let refreshPromise: Promise<boolean> | null = null;

async function refreshTokens(): Promise<boolean> {
    if (refreshPromise) {
        return refreshPromise;
    }

    refreshPromise = (async () => {
        try {
            const url = new URL("auth/refresh", API_URL);

            const response = await fetch(url, {
                method: "POST",
                credentials: "include",
                signal: AbortSignal.timeout(5000),
            });

            return response.ok;
        } catch {
            return false;
        } finally {
            refreshPromise = null;
        }
    })();

    return refreshPromise;
}

export async function apiClient<T>(endpoint: string, options?: RequestInit): Promise<T> {
    return apiClientInternal<T>(endpoint, options, false);
}

export async function apiClientInternal<T>(
    endpoint: string, 
    options: RequestInit | undefined, 
    isRetry: boolean
): Promise<T> {
    const isFormData = options?.body instanceof FormData;
    const url = new URL(endpoint.replace(/^\//, ""), API_URL);

    console.log(`API CALL: ${url}`);

    try {
        const response = await fetch(url, {
            ...options,
            credentials: "include",
            headers: {
                ...(!isFormData && { "Content-Type": "application/json" }),
                ...options?.headers,
            },
            signal: AbortSignal.timeout(5000)
        });

        if (response.status === 401 && !isRetry) {
            const refreshed = await refreshTokens();

            if (refreshed) {
                return apiClientInternal<T>(endpoint, options, true);
            }

            authStore.clearUser();
        }

        const text = await response.text();

        let body: unknown;

        if (text) {
            const contentType = response.headers.get("content-type");

            if (contentType?.includes("json")) {
                try {
                    body = JSON.parse(text);
                } catch {
                    body = undefined;
                }
            }
        }

        if (!response.ok) {
            throw new ApiError(response.status, body as ProblemDetails | undefined);
        }

        return body as T;
    } catch (error) {
        if (error instanceof DOMException && error.name === "TimeoutError") {
            throw new Error("Server takes too long to respond");
        }
        if (error instanceof ApiError) {
            throw error;
        }
        throw new Error("Server is not responding", {
            cause: error
        });
    }
}