import type { ValidationErrors } from "../api/client";

export default function ValidationErrorsList({errors} : {errors: ValidationErrors}) {
    return (
        <ul>
            {Object.entries(errors).flatMap(([field, messages]) =>
                messages.map((message, index) => (
                    <li key={`${field}-${index}`}>
                        {message}
                    </li>
                ))
            )}
        </ul>
    )
}