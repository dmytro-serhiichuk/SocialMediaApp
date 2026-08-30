import { Form } from "react-router";

export interface SettingProfileProps {
    userName: string,
    userTag: string
}

export function SettingsProfile({userName, userTag}: SettingProfileProps) {
    return (
        <>
            <h3>Profile:</h3>

            {/* TODO: implement form */}
            <Form>
                <label>
                    User Name
                    <input name="userName" value={userName}/>
                </label>
                <label>
                    User Tag
                    <input name="userTag" value={userTag}/>
                </label>

                <button type="submit">Update</button>
            </Form>
        </>
    );
}