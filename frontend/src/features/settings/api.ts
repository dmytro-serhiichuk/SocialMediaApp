import { apiClient } from "../../api/client";
import type { EmailChangeConfirmation } from "./types";

export async function confirmEmailChange(token: string) {
    var req: EmailChangeConfirmation = {
        token: token,
    };

    await apiClient("settings/email-change/confirm", { 
        method: "POST",
        body: JSON.stringify(req)
    });
}