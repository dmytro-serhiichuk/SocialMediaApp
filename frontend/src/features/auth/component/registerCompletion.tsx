import { Form, Link, useActionData, useNavigation } from "react-router";
import type { completeRegisterAction } from "../actions";
import { ROUTES } from "../../../constants/routes";
import ValidationErrorsList from "../../../components/validationErrorsList";

export interface RegisterCompletionToken {
    token: string
}

export default function RegisterCompletionComponent({token}: RegisterCompletionToken) {
    const actionData = useActionData<typeof completeRegisterAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";

    if (actionData?.validationErrors?.email) {
        return (
            <>
                <p>User with this email already exists</p>
                <Link to={ROUTES.LOGIN}>Login</Link>
            </>
        );
    }
    
    return (
        <>
            <p>The tag you entered before is not available anymore</p>
            <p>Enter new user tag</p>

            <Form method="POST">
                <input type="hidden" name="token" value={token} />
                
                <label>
                    User Tag
                    <input name="userTag"/>
                </label>

                {actionData?.error && (
                    <p>{actionData.error}</p>
                )}
                {actionData?.validationErrors && (
                    <ValidationErrorsList errors={actionData?.validationErrors} />
                )}

                <button type="submit" disabled={isSubmitting}>
                    Register
                </button>
            </Form>
        </>
    );
}