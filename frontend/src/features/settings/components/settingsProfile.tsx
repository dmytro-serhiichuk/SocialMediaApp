import { Form, useActionData, useNavigation } from "react-router";
import type { settingsAction } from "../actions";
import { SETTINGS_INTENTS } from "../types";

export interface SettingProfileProps {
    userName: string,
    userTag: string
}

export function SettingsProfile({userName, userTag}: SettingProfileProps) {
    const actionData = useActionData<typeof settingsAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";
    
    return (
        <>
            <h3>Profile:</h3>

            <Form method="post">
                <input type="hidden" name="intent" value={SETTINGS_INTENTS.PROFILE_CHANGE} />

                <label>
                    User Name
                    <input name="userName" defaultValue={userName}/>
                </label>
                <label>
                    User Tag
                    <input name="userTag" defaultValue={userTag}/>
                </label>

                {
                    actionData?.intent === SETTINGS_INTENTS.PROFILE_CHANGE && 
                    actionData?.error && 
                    <p>{actionData.error}</p>
                }

                <button type="submit" disabled={isSubmitting}>
                    {isSubmitting ? "Updating..." : "Update"}
                </button>
            </Form>
        </>
    );
}