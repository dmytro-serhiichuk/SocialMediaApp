import { apiClient, ApiError } from "./client";

export function useToggleApiRequest(setIsBusy: (state: boolean) => void) {
    const toggleApiRequest = async (endpoint: string, method: string, func: () => void) => {
        try {
            setIsBusy(true);

            await apiClient(endpoint, {
                method: method
            });

            func();
        } catch (error) {
            if (error instanceof ApiError) {
                alert(error.message);
            } else {
                throw error;
            }
        } finally {
            setIsBusy(false);
        }
    }

    return { toggleApiRequest };
}