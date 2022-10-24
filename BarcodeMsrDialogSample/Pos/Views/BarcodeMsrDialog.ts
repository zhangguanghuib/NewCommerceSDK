import { ExtensionTemplatedDialogBase, ITemplatedDialogOptions } from "PosApi/Create/Dialogs";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";
import { IAlphanumericNumPad, IAlphanumericNumPadOptions } from "PosApi/Consume/Controls";
import { BarcodeMsrDialogInputType, IBarcodeMsrDialogResult } from "./BarcodeMsrDialogTypes";

type BarcodeMsrDialogResolveFunction = (value: IBarcodeMsrDialogResult) => void;
type BarcodeMsrDialogRejectFunction = (reason: any) => void;

export default class BarcodeMsrDialog extends ExtensionTemplatedDialogBase {

    public numPad: IAlphanumericNumPad;

    private resolve: BarcodeMsrDialogResolveFunction;
    private _inputType: BarcodeMsrDialogInputType;
    private _automateEntryInProgress: boolean;

    constructor() {
        super();

        this._inputType = "None";
        this._automateEntryInProgress = false;

        this.onBarcodeScanned = (data: string): void => {
            this._automateEntryInProgress = true;
            this.numPad.value = data;
            this._inputType = "Barcode";
            this._automateEntryInProgress = false;
        };

        this.onMsrSwiped = (data: ClientEntities.ICardInfo): void => {
            this._automateEntryInProgress = true;
            this.numPad.value = data.CardNumber;
            this._inputType = "MSR";
            this._automateEntryInProgress = false;
        }
    }

    public onReady(element: HTMLElement): void {
        let numPadOptions: IAlphanumericNumPadOptions = {
            globalInputBroker: this.numPadInputBroker,
            label: "Please enter a value, scan or swipe",
            value: ""
        };

        let numPadRootElem: HTMLDivElement = element.querySelector("#barcodeMsrDialogAlphanumbericNumpad") as HTMLDivElement;
        this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "AlphanumericNumPad", numPadOptions, numPadRootElem);
        this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
            this._resolvePromise({ canceled: false, inputType: this._inputType, value: eventData.value.toString() });
        });

        this.numPad.addEventListener("ValueChanged", (eventData: { value: string }) => {
            if (!this._automateEntryInProgress) {
                this._inputType = "Manual";
            }
        });
    }

    public open(): Promise<IBarcodeMsrDialogResult> {
        let promise: Promise<IBarcodeMsrDialogResult> = new Promise((resolve: BarcodeMsrDialogResolveFunction, reject: BarcodeMsrDialogRejectFunction) => {
            this.resolve = resolve;
            let option: ITemplatedDialogOptions = {
                title: "Barcode Scanner and MSR Swipe Dialog",
                onCloseX: this._cancelButtonClickHandler.bind(this)
            };

            this.openDialog(option);
        });

        return promise;
    }

    private _cancelButtonClickHandler(): boolean {
        this._resolvePromise({ canceled: true });
        return false;
    }

    private _resolvePromise(result: IBarcodeMsrDialogResult): void {
        if (ObjectExtensions.isFunction(this.resolve)) {
            this.resolve(result);

            this.resolve = null;
            this.closeDialog();
        }
    }

}