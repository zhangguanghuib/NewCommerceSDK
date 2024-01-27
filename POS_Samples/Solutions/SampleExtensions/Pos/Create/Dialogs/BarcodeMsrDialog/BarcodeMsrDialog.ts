import * as Dialogs from "PosApi/Create/Dialogs";
import { BarcodeMsrDialogInputType, IBarcodeMsrDialogResult } from "./BarcodeMsrDialogTypes";
import { IAlphanumericNumPad, IAlphanumericNumPadOptions } from "PosApi/Consume/Controls";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";

type BarcodeMsrDialogResolveFunction = (value: IBarcodeMsrDialogResult) => void;
type BarcodeMsrDialogRejectFunction = (reason: any) => void;

export default class BarcodeMsrDialog extends Dialogs.ExtensionTemplatedDialogBase {

    public numPad: IAlphanumericNumPad;
    public resolve: BarcodeMsrDialogResolveFunction;
    private _inputType: BarcodeMsrDialogInputType;
    private _automatedEntryInProgress: boolean;

    constructor() {
        super();

        this._inputType = "None";
        this._automatedEntryInProgress = false;

        this.onBarcodeScanned = (data: string): void => {
            this._automatedEntryInProgress = true;
            this.numPad.value = data;
            this._inputType = "Barcode";
            this._automatedEntryInProgress = false;
        }

        this.onMsrSwiped = (data: ClientEntities.ICardInfo): void => {
            this._automatedEntryInProgress = true;
            this.numPad.value = data.CardNumber;
            this._inputType = "MSR";
            this._automatedEntryInProgress = false;

        }
    }

    public onReady(element: HTMLElement): void {
        let numPadOptions: IAlphanumericNumPadOptions = {
            globalInputBroker: this.numPadInputBroker,
            label: "Please enter a value, scan or swipe",
            value: ""
        };

        let numPadRootElem: HTMLDivElement = element.querySelector("#barcodeMsrDialogAlphanumericNumpad") as HTMLDivElement;
        this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "AlphanumericNumPad", numPadOptions, numPadRootElem);
        this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
            this._resolvePromise({ canceled: false, inputType: this._inputType, value: eventData.value.toString() });
        });

        this.numPad.addEventListener("ValueChanged", (eventData: { value: string }) => {
            if (!this._automatedEntryInProgress) {
                this._inputType = "Manual";
            }
        });
    }

    public open(): Promise<IBarcodeMsrDialogResult> {
        let promise: Promise<IBarcodeMsrDialogResult> = new Promise((resolve: BarcodeMsrDialogResolveFunction, reject: BarcodeMsrDialogRejectFunction) => {
            this.resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
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
