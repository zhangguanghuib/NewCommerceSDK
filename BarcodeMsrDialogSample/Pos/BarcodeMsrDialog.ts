import { ExtensionTemplatedDialogBase, ITemplatedDialogOptions } from "PosApi/Create/Dialogs";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";
import { IAlphanumericNumPad, IAlphanumericNumPadOptions } from "PosApi/Consume/Controls";
import { BarcodeMsrDialogInputType, IBarcodeMsrDialogResult } from "./BarcodeMsrDialogTypes";

type BarcodeMsrDialogResolveFunction = (value: IBarcodeMsrDialogResult) => void;
type BarcodeMsrDialogREjectFunction = (reason: any) => void;

export default class BarcodeMsrDialog extends ExtensionTemplatedDialogBase {

    public numPad: IAlphanumericNumPad;

    onReady(element: HTMLElement): void {
        throw new Error("Method not implemented.");
    }

}