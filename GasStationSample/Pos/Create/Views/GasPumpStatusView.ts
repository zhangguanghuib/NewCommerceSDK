import ko from "knockout";
import { IDataList, DataListInteractionMode } from "PosApi/Consume/Controls";
import { Entities } from "../../DataService/DataServiceEntities.g";
import { GasPumpStatus } from "../../GasStationTypes";

import { CustomViewControllerBase, CustomViewControllerExecuteCommandArgs, ICommand, Icons, ICustomViewControllerConfiguration, ICustomViewControllerContext } from "PosApi/Create/Views";
import { ArrayExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import { NumberFormattingHelper } from "NumberFormattingHelper";

export default class GasPumpStatusView extends CustomViewControllerBase {
    public readonly isDetailsPanelVisible: ko.Observable<boolean>;
    public readonly isGasPumpSelected: ko.Computed<boolean>;
    public readonly selectedPumpDescription: ko.Computed<string>;
    public readonly selectedPumpStatus: ko.Computed<string>;
    public readonly selectedGasPump: ko.Observable<Entities.GasPump>;
    public readonly isSelectedGasPumpPumping: ko.Computed<boolean>;
    public readonly selectedGasPumpTotal: ko.Computed<string>;
    public readonly selectedGadPumpVolume: ko.Computed<string>;

    private static readonly DATA_LIST_QUERY_SELECTOR: string = "#contoso_gasPumpStatusView_DataList";
    private static readonly STOP_ALL_COMMADN_NAME: string = "StopAllPumps";
    private static readonly START_ALL_COMMAND_NAME: string = "StartALlPumps";
    private static readonly CHECKOUT_COMMAND_NAME: string = "Checkout";
    private static readonly START_STOP_COMMAND_NAME: string = "StartStopPumpCommand";

    private _dataList: IDataList<Entities.GasPump>;
    private _gasPumpChangedHandlerId: number;

    constructor(context: ICustomViewControllerContext) {
        let config: ICustomViewControllerConfiguration = {
            title: "Pump Status View",
            commandBar: {
                commands: [
                    {
                        name: GasPumpStatusView.CHECKOUT_COMMAND_NAME,
                        label: "Checkout",
                        icon: Icons.Add,
                        isVisible: true,
                        canExecute: false,
                        execute: (args: CustomViewControllerExecuteCommandArgs): void => {
                            //this._addGasolineToCartAsync();
                        }
                    },
                    {
                        name: GasPumpStatusView.START_STOP_COMMAND_NAME,
                        label: "Enable/Disable",
                        icon: Icons.LightningBolt,
                        isVisible: true,
                        canExecute: false,
                        execute: (args: CustomViewControllerExecuteCommandArgs): void => {
                            //this._enableDisablePumpAsync();
                        }
                    },
                    {
                        name: GasPumpStatusView.STOP_ALL_COMMADN_NAME,
                        label: "Disable all pumps",
                        icon: Icons.Cancel,
                        isVisible: true,
                        canExecute: true,
                        execute: (args: CustomViewControllerExecuteCommandArgs): void => {
                            //Place holder
                        }
                    },
                    {
                        name: GasPumpStatusView.START_ALL_COMMAND_NAME,
                        label: "Enable all pumps",
                        icon: Icons.Go,
                        isVisible: true,
                        canExecute: false,
                        execute: (args: CustomViewControllerExecuteCommandArgs): void => {
                            //Place holder
                        }
                    },
                    {
                        name: "ShowGasStationDetails",
                        label: "Show station information",
                        icon: Icons.DeliveryTruck,
                        canExecute: true,
                        isVisible: true,
                        execute: (args: CustomViewControllerExecuteCommandArgs): void => {
                            //Place holder
                        }
                    }
                ]
            }
        }

        super(context, config);

        this.isDetailsPanelVisible = ko.observalble(true);
        this.selectedGasPump = ko.observable(undefined);
        this.isGasPumpSelected = ko.computed((): boolean => {
            return !ObjectExtensions.isNullOrUndefined(this.selectedGasPump())
        }, this);

        this.selectedPumpDescription = ko.computed((): string => {
            return ObjectExtensions.isNullOrUndefined(this.selectedGasPump()) ? "" : this.selectedGasPump().Description;
        }, this);

        this.selectedPumpStatus = ko.computed((): string => {
            if (ObjectExtensions.isNullOrUndefined(this.selectedGasPump())) {
                return "";
            }

            let status: GasPumpStatus = this.selectedGasPump().State.GasPumpStatusValue;
            switch (status) {
                case GasPumpStatus.Idle:
                    return "Idle";
                case GasPumpStatus.Stopped:
                    return "Pump Disabled";
                case GasPumpStatus.Pumping:
                    return "Pumping complete";
                case GasPumpStatus.Emergency:
                    return "Pump Emergency";
                default:
                    return "Unknown status";
            }
        }, this);

        this.isSelectedGasPumpPumping = ko.computed((): boolean => {
            if (!this.isGasPumpSelected()) {
                return false;
            }

            return this.selectedGasPump().State.GasPumpStatusValue === GasPumpStatus.Pumping ||
                this.selectedGasPump().State === GasPumpStatus.PumpingComplete;
        }, this);

        this.selectedGasPumpTotal = ko.computed((): string => {
            if (ObjectExtensions.isNullOrUndefined(this.selectedGasPump())) {
                return CurrencyFormatter.toCurrency(0);
            }

            let total: number = this.selectedGasPump().State.SalesTotal;
            if (ObjectExtensions.isNullOrUndefined(total) || isNaN(total)) {
                total = 0;
            }

            return CurrencyFormatter.toCurrency(total);
        }, this);

        this.selectedGadPumpVolume = ko.computed((): string => {
            if (ObjectExtensions.isNullOrUndefined(this.selectedGasPump())) {
                return "0.000";
            }

            let volume: number = this.selectedGasPump().State.SaleVolume;
            if (ObjectExtensions.isNullOrUndefined(volume) || isNaN(volume)) {
                volume = 0;
            }
            return NumberFormattingHelper.getRoundedStringValue(volume, 3);
        }, this);
    }

    private get _stopAllCommand(): ICommand {
        return ArrayExtensions.firstOrUndefined(this.state.commandBar.commands, (c) => c.name === GasPumpStatusView.STOP_ALL_COMMADN_NAME);
    }

    private get _startAllComand(): ICommand {
        return ArrayExtensions.firstOrUndefined(this.state.commandBar.commands, (c) => c.name === GasPumpStatusView.START_ALL_COMMAND_NAME);
    }

    private get _checkoutCommand(): ICommand {
        return ArrayExtensions.firstOrUndefined(this.state.commandBar.commands, (c) => c.name === GasPumpStatusView.CHECKOUT_COMMAND_NAME);
    }

    private get _toggleStartStopCommand(): ICommand {
        return ArrayExtensions.firstOrUndefined(this.state.commandBar.commands, (c) => c.name === GasPumpStatusView.START_STOP_COMMAND_NAME);
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
    }

}