import ko from "knockout";

import * as Views from "PosApi/Create/Views";
import * as Controls from "PosApi/Consume/Controls";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class NumericNumPadView extends Views.CustomViewControllerBase {
    public numPad: Controls.INumericNumPad;
    public numPadValue: ko.Observable<string>;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);
        this.state.title = "NumericNumPad sample";

        this.numPadValue = ko.observable("");
    }

    /**
     * Bind the html element with view controller.
     *
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        //Initialize numpad
        let inputBroker: Commerce.Peripherals.INumPadInputBroker = null;
        let numPadOptions: Controls.INumericNumPadOptions = {
            decimalPrecision: 1,
            globalInputBroker: inputBroker,
            label: "NumPad label",
            value: 0
        };

        let numPadRootElem: HTMLDivElement = element.querySelector("#NumericNumPad") as HTMLDivElement;
        this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "NumericNumPad", numPadOptions, numPadRootElem);
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

    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}