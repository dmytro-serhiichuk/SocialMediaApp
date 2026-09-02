import { Form, useActionData, useNavigation } from "react-router";
import { SETTINGS_INTENTS } from "../types";
import type { settingsAction } from "../actions";

export interface SettingEmailProps {
    email: string
}

export function SettingsEmail({email}: SettingEmailProps) {
    const actionData = useActionData<typeof settingsAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";
    
    return (
        <>
            <h3>Email:</h3>
            
            <Form method="post">
                <input type="hidden" name="intent" value={SETTINGS_INTENTS.EMAIL_CHANGE} />
                
                <label>
                    Email
                    <input name="email" type="email" defaultValue={email}/>
                </label>

                {
                    actionData?.intent === SETTINGS_INTENTS.EMAIL_CHANGE && 
                    actionData?.error && 
                    <p>{actionData.error}</p>
                }

                <p>After submitting a confirmation letter will be sent to your new email</p>
                <button type="submit" disabled={isSubmitting}>
                    {isSubmitting ? "Updating..." : "Update"}
                </button>
            </Form>
        </>
    );
}