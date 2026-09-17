export type InputImageInfo =
    | {
        type: "existing",
        id: string,
        url: string
    }
    | {
        type: "new",
        file: File
    }

export interface InputImageInfoRequest {
    imageId?: string,
    index?: number
};