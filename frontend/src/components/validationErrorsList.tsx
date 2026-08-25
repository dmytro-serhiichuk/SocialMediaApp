export default function ValidationErrorsList({errors} : {errors: Record<string, string[]>}) {
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