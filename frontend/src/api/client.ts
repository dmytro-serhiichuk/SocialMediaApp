const API_URL = "http://localhost:5126/api/";

type ProblemDetails = {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
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
}

export async function apiClient<T>(endpoint: string, options?: RequestInit): Promise<T> {
    console.log("API CALL");

    const isFormData = options?.body instanceof FormData;
    const url = new URL(endpoint.replace(/^\//, ""), API_URL);

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
        throw new Error("Server is not responding");
    }
}