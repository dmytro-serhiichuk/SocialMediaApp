import { Form, useActionData, useNavigation } from "react-router";
import { SETTINGS_INTENTS } from "../types";
import type { settingsAction } from "../actions";

type SettingsAddPasswordProps = {
    email: string
}

export default function SettingsPasswordCreate({email}: SettingsAddPasswordProps) {
    const actionData = useActionData<typeof settingsAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";
    
    return (
        <>
            <h3>Password:</h3>

            {(actionData?.intent === SETTINGS_INTENTS.PASSWORD_CREATE && actionData.success) ?
                <p>Check your email to follow a password reset link</p> :
                <>
                    <p>Your account doesn't have a password. You can press this button to receive a password creation link on your email.</p>

                    <Form method="post">
                        <input type="hidden" name="intent" value={SETTINGS_INTENTS.PASSWORD_CREATE} />
                        <input type="hidden" name="email" value={email} />

                        {
                            actionData?.intent === SETTINGS_INTENTS.PASSWORD_CREATE && 
                            actionData?.error && 
                            <p>{actionData.error}</p>
                        }
                        
                        <button 
                            type="submit"
                            disabled={isSubmitting}>
                            Add Password
                        </button>  
                    </Form>
                </>
            }

        </>
    );
}