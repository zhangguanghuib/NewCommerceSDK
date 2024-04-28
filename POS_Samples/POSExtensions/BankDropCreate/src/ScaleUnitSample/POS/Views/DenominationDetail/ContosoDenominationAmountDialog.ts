import * as Dialogs from "PosApi/Create/Dialogs";
import * as Controls from "PosApi/Consume/Controls";
import ko from "knockout";
import { IContosoDenominationInputDialogResult } from "./ContosoDenominationNumericDialogTypes";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import ContosoNumberExtensions from "../ContosoTenderCounting/ContosoNumberExtensions";

type ContosoDenominationInputDialogResolveFunction = (value: IContosoDenominationInputDialogResult) => void;
type ContosoDenominationInputDialogRejectFunction = (reason: any) => void;

export default class ContosoDenominationAmountDialog extends Dialogs.ExtensionTemplatedDialogBase {
    public _item: ProxyEntities.DenominationDetail;

    public numPad: Controls.ICurrencyNumPad;
    public numPadValue: ko.Observable<string>;

    public currency: ko.Observable<string>;
    public denominationLabel: ko.Observable<string>;
    public denominationValue: ko.Observable<string>;
    public denominationTotalValue: ko.Observable<string>;

    public _numPadOptions: Controls.ICurrencyNumPadOptions;
    public resolve: ContosoDenominationInputDialogResolveFunction;

    constructor() {
        super();
        this.numPadValue = ko.observable("");

        this.currency = ko.observable('USD');
        this.denominationLabel = ko.observable('1.00');
        this.denominationValue = ko.observable('2.00');
        this.denominationTotalValue = ko.observable('4.00');
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        let inputBroker: Commerce.Peripherals.INumPadInputBroker = null;
        let numPadOptions: Controls.ICurrencyNumPadOptions = {
            currencyCode: "USD",
            globalInputBroker: inputBroker,
            label: "Enter Amount",
            value: 0
        };
        this._numPadOptions = numPadOptions;
        let numPadRootElem: HTMLDivElement = element.querySelector("#CurrencyNumPad") as HTMLDivElement;
        this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "CurrencyNumPad", numPadOptions, numPadRootElem);
        this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
            return this._resolvePromise({ canceled: false, item: this._item, value: Number(eventData.value.toString()) });
        });

        this.numPad.addEventListener("ValueChanged", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
            // Update UI
            let denominationAmount: number = ContosoNumberExtensions.roundToNDigits(Number(eventData.value.toString()), 2);
            this.denominationTotalValue(CurrencyFormatter.toCurrency(denominationAmount));
            let denominationQty: number = ContosoNumberExtensions.roundToNDigits(denominationAmount / this._item.DenominationAmount, 0);
            this.denominationValue(CurrencyFormatter.toCurrency(denominationQty));

            // Update original item
            this._item.QuantityDeclared = denominationQty;
            this._item.AmountDeclared = denominationAmount;
        });
    }


    public open(inputDialogResult: IContosoDenominationInputDialogResult): Promise<IContosoDenominationInputDialogResult> {
        let promise: Promise<IContosoDenominationInputDialogResult> = new Promise((resolve: ContosoDenominationInputDialogResolveFunction, reject: ContosoDenominationInputDialogRejectFunction) => {
            this.resolve = resolve;
            this._item = inputDialogResult.item;

            this.currency(inputDialogResult.item.Currency);
            this.denominationLabel = ko.observable(inputDialogResult.item.DenominationAmount);
            this.denominationValue = ko.observable(CurrencyFormatter.toCurrency(inputDialogResult.item.QuantityDeclared));
            this.denominationTotalValue = ko.observable(CurrencyFormatter.toCurrency(inputDialogResult.item.AmountDeclared));

            let option: Dialogs.ITemplatedDialogOptions = {
                title: inputDialogResult.inputType,
                onCloseX: this._cancelButtonClickHandler.bind(this)
            };
            this.openDialog(option);
        });

        return promise;
    }


    private _cancelButtonClickHandler(): boolean {
        this._resolvePromise({ canceled: true });
        return false;
    }

    private _resolvePromise(result: IContosoDenominationInputDialogResult): Promise<IContosoDenominationInputDialogResult> {
        if (ObjectExtensions.isFunction(this.resolve)) {
            this.resolve(result);

            this.resolve = null;
            this.closeDialog();
            return Promise.resolve(result);
        } else {
            return Promise.resolve({ canceled: true });
        }
    }

}