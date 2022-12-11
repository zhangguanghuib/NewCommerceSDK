import { CustomPackingItem, CustomPackingItemPosition, ICustomPackingItemContext } from "PosApi/Extend/Header";

import { Entities } from "../../DataService/DataServiceEntities.g";

import ko from "knockout";
import { GasPumpStatus } from "GasStationTypes";

import GasPump = Entities.GasPump;
import { PumpStatusChangedHandler } from "GasStationDataStore";

export default class GasPumpStatusPackingItem extends CustomPackingItem {


    public readonly position: Commerce.Extensibility.CustomPackingItemPosition = CustomPackingItemPosition.After;

    public backgroundColor: ko.Computed<string>;

    public label: ko.Computed<string>;

    private static readonly OK_STATUS_COLOR: string = "green";
    private static readonly WARNING_STATUS_COLOR: string = "orange";
    private static readonly EMERGENCY_STATUS_COLOR: string = "red";
    private static readonly OK_STATUS_LABEL: string = "Pump Statues: OK";
    private static readonly WARNING_STATUS_LABEL: string = "Pump Statuses: Warning";
    private static readonly EMERGENCY_STATUS_LABEL: string = "Pump Statuses: Emergency";

    private _overallStatus: ko.Observable<GasPumpStatus>;
    private _pumps: GasPump[];
    private _pumpStatusChangedHandlerProxied: PumpStatusChangedHandler;

    constructor(id: string, context: ICustomPackingItemContext) {
        super(id, context);

        this._overallStatus = ko.observable(GasPumpStatus.Unknown);

        this.backgroundColor = ko.computed((): string => {
            switch (this._overallStatus()) {
                case GasPumpStatus.Emergency:
                    return GasPumpStatusPackingItem.EMERGENCY_STATUS_COLOR;
                case GasPumpStatus.Stopped:
                    return GasPumpStatusPackingItem.WARNING_STATUS_COLOR;
                default:
                    return GasPumpStatusPackingItem.OK_STATUS_COLOR;
            }
        }, this);

        this.label = ko.computed((): string => {
            switch (this._overallStatus()) {
                case GasPumpStatus.Emergency:
                    return GasPumpStatusPackingItem.EMERGENCY_STATUS_LABEL;
                case GasPumpStatus.Stopped:
                    return GasPumpStatusPackingItem.WARNING_STATUS_LABEL;
                default:
                    return GasPumpStatusPackingItem.OK_STATUS_LABEL;
            }
        }, this);
    }

    public onReady(packedElement: HTMLElement, unpackedElement: HTMLElement): void {
        
    }

    protected init(state: Commerce.Extensibility.ICustomHeaderPackingItemState): void {
        
    }

}