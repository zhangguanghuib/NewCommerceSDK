export interface IImageCaptureDialogResult {
    imageData?: string, // Base64 string from canvas.toDataURL()
    description?: string,
    timestamp?: string
}