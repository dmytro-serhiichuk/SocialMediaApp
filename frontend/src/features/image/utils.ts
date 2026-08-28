import { SERVER_URL } from "../../constants";

export function getFullImageUrl(endpoint: string) {
    return `${SERVER_URL}${endpoint}`;
}