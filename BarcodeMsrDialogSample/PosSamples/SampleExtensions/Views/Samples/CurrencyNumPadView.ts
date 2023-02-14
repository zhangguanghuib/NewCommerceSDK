import ko from "knockout";


import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import * as Controls from "PosApi/Consume/Controls";

export default class CurrencyNumPadView extends Views.CustomViewControllerBase {
    public numPad: Controls.ICurrencyNumPad;
    public numPadValue: ko.Observable<string>;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);
        this.state.title = "CurrencyNumPad sample";

        this.numPadValue = ko.observable("");
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        //Initialize numpad
        let inputBroker: Commerce.Peripherals.INumPadInputBroker = null;
        let numPadOptions: Controls.ICurrencyNumPadOptions = {
            currencyCode: "USD",
            globalInputBroker: inputBroker,
            label: "NumPad label",
            value: 0
        };

        let numPadRootElem: HTMLDivElement = element.querySelector("#CurrencyNumPad") as HTMLDivElement;
        this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "CurrencyNumPad", numPadOptions, numPadRootElem);
        this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
            this.onNumPadEnter(eventData.value);
        });
    }

    /**
     * Callback for numpad.
     * @param {number} value Numpad current value.
     */
    private onNumPadEnter(value: Commerce.Extensibility.NumPadValue): void {
        this.numPadValue(value.toString());
        this.numPad.value = 0;
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}