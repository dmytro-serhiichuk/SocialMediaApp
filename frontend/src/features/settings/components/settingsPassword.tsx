import { Form } from "react-router";

export function SettingsPassword() {
    return (
        <>
            <h3>Password:</h3>

            {/* TODO: implement form */}
            <Form>
                <label>
                    Password
                    <input name="password" type="password"/>
                </label>
                <label>
                    Repeat Password
                    <input name="password-confirmation" type="password"/>
                </label>
            </Form>
        </>
    );
}