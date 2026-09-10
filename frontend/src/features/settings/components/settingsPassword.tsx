import { Form, useActionData, useNavigation } from "react-router";
import { SETTINGS_INTENTS } from "../types";
import type { settingsAction } from "../actions";
import { useRef, useState } from "react";

export function SettingsPassword() {
    const actionData = useActionData<typeof settingsAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";

    const password1Ref = useRef<HTMLInputElement>(null);
    const password2Ref = useRef<HTMLInputElement>(null);
    const [valid, setValid] = useState<boolean>(false);

    const handleChange = () => {
        if (password1Ref.current && password2Ref.current) {
            // TODO: add password validation
            
            const val1 = password1Ref.current.value;
            const val2 = password2Ref.current.value;
            
            setValid(val1.trim().length > 0 && val1 === val2);
        }
    }

    // TODO: handle password creation if account doen't have a password

    return (
        <>
            <h3>Password:</h3>

            <Form method="POST">
                <input type="hidden" name="intent" value={SETTINGS_INTENTS.PASSWORD_CHANGE} />

                {/* TODO: add "eye" button */}
                {/* TODO: clear inputs after successful change */}
                <label>
                    Current password
                    <input name="old-password" type="password" onChange={handleChange}/>
                </label>
                <label>
                    New password
                    <input name="new-password" type="password" ref={password1Ref} onChange={handleChange}/>
                </label>
                <label>
                    Confirm new password
                    <input type="password" ref={password2Ref} onChange={handleChange}/>
                </label>

                {/* TODO: show message if new password confirmation is incorrect */}

                {
                    actionData?.intent === SETTINGS_INTENTS.PASSWORD_CHANGE && 
                    actionData?.error && 
                    <p>{actionData.error}</p>
                }

                <button type="submit" disabled={isSubmitting || !valid}>
                    {isSubmitting ? "Updating..." : "Update"}
                </button>
            </Form>
        </>
    );
}