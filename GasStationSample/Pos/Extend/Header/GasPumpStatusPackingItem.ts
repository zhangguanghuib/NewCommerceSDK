import { CustomPackingItem, CustomPackingItemPosition, ICustomPackingItemContext } from "PosApi/Extend/Header";

import { Entities } from "../../DataService/DataServiceEntities.g";

import ko from "knockout";
import { GasPumpStatus } from "GasStationTypes";

import GasPump = Entities.GasPump;
import { GasStationDataStore, PumpStatusChangedHandler } from "GasStationDataStore";

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
        ko.applyBindingsToNode(unpackedElement, {
            template: {
                name: "Microsoft_Pos_GasStationHeaderExtensionSample_UnpackedGasPumpStatusItem",
                data: this
            }
        });

        ko.applyBindingsToNode(packedElement, {
            template: {
                name: "Microsoft_PosGasStationHeaderExtensionSample_PackedGasPumpStatusItem",
                data: this
            }
        });
    }

    protected init(state: Commerce.Extensibility.ICustomHeaderPackingItemState): void {
        this.visible = true;
        this._pumps = GasStationDataStore.instance.pumps;
        this._reevaluatePumpStatuses();
        this._pumpStatusChangedHandlerProxied = (pumps: GasPump[]): void => {
            this._pumps = pumps;
            this._reevaluatePumpStatuses();
        }

        GasStationDataStore.instance.addPumpStatusChangedHandler(this._pumpStatusChangedHandlerProxied);
        
    }

    public dispose(): void {
        super.dispose();
    }

    public onItemClickedHandler(): void {
        this.context.navigator.navigate("GasPumpStatusView");
    }

    private _reevaluatePumpStatuses(): void {
        const newStatus: GasPumpStatus = this._getGeneralPumpsStatus(this._pumps);
        this._overallStatus(newStatus);
    }

    private _getGeneralPumpsStatus(pumps: GasPump[]): GasPumpStatus {
        let newStatus: GasPumpStatus = GasPumpStatus.Unknown;

        for (let i = 0; i < pumps.length; ++i) {
            const pumpState: GasPumpStatus = pumps[i].State.GasPumpStatusValue;
            switch (pumpState) {
                case GasPumpStatus.Emergency:
                    return GasPumpStatus.Emergency;
                case GasPumpStatus.Stopped:
                    newStatus = GasPumpStatus.Stopped;
                    break;
                default:
                    break;
            }
        }

        return newStatus;
    }

}