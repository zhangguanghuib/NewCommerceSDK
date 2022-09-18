import * as Dialogs from "PosApi/Create/Dialogs";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { GasStationDataStore } from "../../GasStationDataStore";
import { Entities } from "DataService/DataServiceEntities.g";
import { DateFormatter, CurrencyFormatter } from "PosApi/Consume/Formatters";
import { NumberFormattingHelper } from "../../NumberFormattingHelper";
import ko from "knockout";

export default class GasStationDetailsDialog extends Dialogs.ExtensionTemplatedDialogBase {

    public readonly gasStationDetails: Entities.GasStationDetails;
    public readonly lastGasDeliveryTime: string;
    public readonly nextGasDeliveryTime: string;
    public readonly gasTankCapacity: string;
    public readonly gasTankLevel: string;
    public readonly pricePerUnit: string;
    public readonly pumpCount: string;

    private _resolve: () => void;

    public constructor() {
        super();

        this.gasStationDetails = GasStationDataStore.instance.gasStationDetails;
        this.nextGasDeliveryTime = DateFormatter.toShortDateAndTime(this.gasStationDetails.NextGasDeliveryTime);
        this.lastGasDeliveryTime = DateFormatter.toShortDateAndTime(this.gasStationDetails.LastGasDeliveryTime);
        this.gasTankCapacity = NumberFormattingHelper.getRoundedStringValue(this.gasStationDetails.GasTankCapacity, 2);
        this.gasTankLevel = NumberFormattingHelper.getRoundedStringValue(this.gasStationDetails.GasTankLevel, 2) + " gallons";
        this.pricePerUnit = CurrencyFormatter.toCurrency(NumberFormattingHelper.roundToNdigits(this.gasStationDetails.GasBasePrice, 2)) + " per gallon";
        this.pumpCount = NumberFormattingHelper.getRoundedStringValue(this.gasStationDetails.GasPumpCount, 0);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    public open(): Promise<void> {
        let promise: Promise<void> = new Promise<void>((resolve: () => void, reject: (reason: any) => void): void => {
            this._resolve = resolve;

            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Station Information",
                onCloseX: this.onCloseX.bind(this),
                button1: {
                    id: "OkayButton",
                    label: "OK",
                    isPrimary: true,
                    onClick: this._enablePumpClickHandler.bind(this)
                }
            };
            this.openDialog(option);
        });

        return promise;
    }

    private onCloseX(): boolean {
        this._resolvePromise();
        return true;
    }

    private _enablePumpClickHandler(): boolean {
        this._resolvePromise();
        return true;
    }

    private _resolvePromise(): void {
        if (ObjectExtensions.isFunction(this._resolve)) {
            this._resolve();
            this._resolve = null;
        }
    }

}