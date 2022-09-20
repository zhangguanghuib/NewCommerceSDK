import * as Dialogs from "PosApi/Create/Dialogs";
import { Entities } from "../../DataService/DataServiceEntities.g";
import { ClientEntities } from "PosApi/Entities";
import ko from "knockout";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import { NumberFormattingHelper } from "../../NumberFormattingHelper";
import { GasStationDataStore } from "../../GasStationDataStore";
import { ArrayExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import { GasPumpStatus } from "../../GasStationTypes";

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

    public open(options: { pumpId: number }): Promise<ClientEntities.ICancelableDataResult<number>> {
        this.gasPump = ArrayExtensions.firstOrUndefined(GasStationDataStore.instance.pumps, (pump): boolean => { return pump.Id === options.pumpId });
        if (ObjectExtensions.isNullOrUndefined(this.gasPump)) {
            return Promise.reject(new Error("Pump not found"));
        }

        let intervalId = setInterval(() => {
            let updatedQuantity: number = this._pumpedQuantity() + 0.033;
            this._pumpedQuantity(updatedQuantity);

            if (updatedQuantity > 75) {
                this.isEmergencyInProgress(true);
                GasStationDataStore.instance.updatePumpStatusAsync(this.context, options.pumpId, { GasPumpStatusValue: GasPumpStatus.Emergency, LastUpdateTime: new Date(), SaleTotal: 0, SaleVolume: 0 });
                clearInterval(intervalId);
                intervalId = undefined;
            }
        }, 50);

        let promise: Promise<void> = new Promise<void>((resolve: () => void, reject: (reason: any) => void): void => {
            this._resolve = resolve;
            let option: Dialogs.ITemplatedDialogOptions = {
                title: this.gasPump.Name,
                onCloseX: this.onCloseX.bind(this),
                button1: {
                    id: "OkayButton",
                    label: "Stop pumping",
                    isPrimary: true,
                    onClick: this._stopPumpClickHandler.bind(this)
                }
            };

            this.openDialog(option);
        });

        return promise.then((): ClientEntities.ICancelableDataResult<number> => {
            if (typeof intervalId === "number") {
                clearInterval(intervalId);
            }

            let finalQuantity: number = this._pumpedQuantity();
            return { canceled: this._canceled, data: finalQuantity };
        });
    }


    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    private onCloseX(): boolean {
        this._canceled = true;
        this._resolvePromise();
        return true;
    }

    private _stopPumpClickHandler(): boolean {
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