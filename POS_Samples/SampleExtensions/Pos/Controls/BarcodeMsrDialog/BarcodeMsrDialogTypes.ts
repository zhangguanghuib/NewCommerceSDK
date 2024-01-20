
export type BarcodeMsrDialogInputType = "None" | "Manual" | "Barcode" | "MSR"

export interface IBarcodeMsrDialogResult {
    canceled: boolean;
    inputType?: BarcodeMsrDialogInputType;
    value?: string;
}