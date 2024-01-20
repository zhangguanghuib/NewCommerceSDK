import ko from "knockout";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class MenusView extends Views.CustomViewControllerBase {

    public readonly selectedPumpStatus: ko.Computed<string>;
    public readonly isSelectedGasPumpPumping: ko.Computed<boolean>;
    public readonly selectedGasPumpTotal: ko.Computed<string>;
    public readonly selectedGasPumpVolume: ko.Computed<string>;
    public readonly selectedPumpDescription: ko.Computed<string>;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);

        this.state.title = "Gas Pump Status";
        this.selectedPumpDescription = ko.computed((): string => {
            return "Pump Description";
        }, this);

        this.selectedPumpStatus = ko.computed((): string => {
            return "Pumping complete";
        }, this);


        this.isSelectedGasPumpPumping = ko.computed((): boolean => {
            return true;
        }, this);

        this.selectedGasPumpTotal = ko.computed((): string => {
            let total: number = 125;
            return CurrencyFormatter.toCurrency(total);
        }, this);

        this.selectedGasPumpVolume = ko.computed((): string => {
            return "23.456";
        }, this);
    }


    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }


    onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }
}