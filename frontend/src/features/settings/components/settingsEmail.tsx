import { Form } from "react-router";

export interface SettingEmailProps {
    email: string
}

export function SettingsEmail({email}: SettingEmailProps) {
    return (
        <>
            <h3>Email:</h3>
            
            {/* TODO: implement form */}
            <Form>
                <label>
                    Email
                    <input name="email" type="email" value={email}/>
                </label>

                <p>After submitting a confirmation letter will be sent to your new email</p>
                <button type="submit">Update</button>
            </Form>
        </>
    );
}