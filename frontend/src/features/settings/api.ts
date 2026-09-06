import { apiClient } from "../../api/client";
import type { ConfirmationRequest } from "../confirmation/types";

export async function confirmEmailChange(token: string) {
    var req: ConfirmationRequest = {
        token: token,
    };

    await apiClient("settings/email-change/confirm", { 
        method: "POST",
        body: JSON.stringify(req)
    });
}