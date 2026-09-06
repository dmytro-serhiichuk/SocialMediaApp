import { Form, useActionData, useNavigation } from "react-router";
import type { registerAction } from "../features/auth/actions";
import ValidationErrorsList from "../components/validationErrorsList";

export default function RegisterPage() {
    const actionData = useActionData<typeof registerAction>();
    const navigation = useNavigation();

    const isSubmitting = navigation.state === "submitting";
    
    return (
        <>
            <p>Register page</p>

            {actionData?.success ? 
                <p>Check your email to confirm the registration</p>
                :
                <Form method="post">
                    {/* TODO: add password double check and data validation */}
                    <label>
                        User Name
                        <input name="userName"/>
                    </label>
                    <label>
                        User Tag
                        <input name="userTag"/>
                    </label>
                    <label>
                        Email
                        <input name="email" type="email"/>
                    </label>
                    <label>
                        Password
                        <input name="password" type="password"/>
                    </label>

                    {actionData?.error && (
                        <p>{actionData.error}</p>
                    )}
                    {actionData?.validationErrors && 
                        <ValidationErrorsList errors={actionData?.validationErrors} />
                    }

                    <button 
                        type="submit"
                        disabled={isSubmitting}>
                        Register
                    </button>
                </Form>
            }            
        </>
    );
}