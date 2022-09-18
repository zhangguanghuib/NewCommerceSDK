import * as Dialogs from "PosApi/Create/Dialogs";
import { Entities } from "../../DataService/DataServiceEntities.g";
import ko from "knockout";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import { NumberFormattingHelper } from "../../NumberFormattingHelper";
import { GasStationDataStore } from "../../GasStationDataStore";

export default class PumpGasDialog extends Dialogs.ExtensionTemplatedDialogBase {
    public gasPump: Entities.GasPump;
    public pumpedTotalAmount: ko.Computed<string>;
    public pumpedQuantity: ko.Computed<string>;
    public readonly pricePerUnit: string;
    public isEmergencyInProgress: ko.Observable<boolean>;
    private _pumpedQuantity: ko.Observable<number>;
    private _canceled: boolean;
    private _resolve: () => void;

    public constructor() {
        super();

        this._pumpedQuantity = ko.Observable(0);
        this.pricePerUnit = CurrencyFormatter.toCurrency(NumberFormattingHelper.roundToNdigits(GasStationDataStore.instance.gasStationDetails.GasBasePrice, 2)) + " per gallon";
        this.pumpedQuantity = ko.Computed((): string => {
            return NumberFormattingHelper.getRoundedStringValue(this._pumpedQuantity(), 3);
        }, this);

        this.pumpedTotalAmount = ko.Computed((): string => {
            let totalAmount: number = NumberFormattingHelper.roundToNdigits(this._pumpedQuantity() * GasStationDataStore.instance.gasStationDetails.GasBasePrice, 2);
            return CurrencyFormatter.toCurrency(totalAmount);
        }, this);

        this._canceled = false;
        this.isEmergencyInProgress = ko.Observable(false);
    }


    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

}