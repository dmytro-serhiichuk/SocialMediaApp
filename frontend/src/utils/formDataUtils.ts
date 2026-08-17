export function getFormString(formData: FormData, name: string): string {
    const value = formData.get(name);
    if (typeof value !== "string") throw new Error(`Invalid form field: ${name}`);
    return value;
}

export function getFormStrings(formData: FormData, name: string): string[] {
    return formData.getAll(name).map(value => {
        if (typeof value !== "string") {
            throw new Error(`Expected ${name} to contain strings`);
        }
        return value;
    });
}
