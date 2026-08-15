export function getFormString(formData: FormData, name: string): string {
    const value = formData.get(name);

    if (typeof value !== "string") {
        throw new Error(`Invalid form field: ${name}`);
    }

    return value;
}