
import * as Views from "PosApi/Create/Views";
import * as Controls from "PosApi/Consume/Controls";
import ko from "knockout";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class NumericNumpadView extends Views.CustomViewControllerBase implements Views.INumPadInputSubscriberEndpoint {
    public numPad: Controls.INumericNumPad;
    public numpadValue: ko.Observable<string>;
    public readonly implementsINumPadInputSubscriberEndpoint: true;
    private readonly _numPadOptions: Controls.INumericNumPadOptions;


    constructor(context: Views.ICustomViewControllerContext) {
        super(context);
        this.state.title = "NumericNumPad Sample";

        this.numpadValue = ko.observable("");
        this.implementsINumPadInputSubscriberEndpoint = true;
        this._numPadOptions = {
            decimalPrecision: 0,
            globalInputBroker: undefined,
            label: "Numpad label",
            value: 0
        };
    }

    
    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        let numPadRootElem: HTMLDivElement = element.querySelector("#NumericNumPad") as HTMLDivElement;
        this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "NumericNumPad", this._numPadOptions, numPadRootElem);
        this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
            this.onNumPadEnter(eventData.value);
        })
    }

    private onNumPadEnter(value: Commerce.Extensibility.NumPadValue) {
        this.numpadValue(value.toString());
        this.numPad.value = 0;
    }

    public setNumPadInputSubscriber(numPadInputSubscriber: Commerce.Peripherals.INumPadInputSubscriber): void {
        this._numPadOptions.globalInputBroker = numPadInputSubscriber;
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }


}