
import * as Dialogs from "PosApi/Create/Dialogs";
import * as Views from "PosApi/Create/Views";
import * as Controls from "PosApi/Consume/Controls";
import ko from "knockout";
import { IContosoDenominationInputDialogResult } from "./ContosoDenominationNumericDialogTypes";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import ContosoNumberExtensions from "../ContosoTenderCounting/ContosoNumberExtensions";

type ContosoDenominationInputDialogResolveFunction = (value: IContosoDenominationInputDialogResult) => void;
type ContosoDenominationInputDialogRejectFunction = (reason: any) => void;
export default class ContosoDenominationInputDialog extends Dialogs.ExtensionTemplatedDialogBase {
    public _item: ProxyEntities.DenominationDetail;
    public numPad: Controls.INumericNumPad;
    public numpadValue: ko.Observable<string>;
    private readonly _numPadOptions: Controls.INumericNumPadOptions;

    public currency: ko.Observable<string>;
    public denominationLabel: ko.Observable<string>;
    public denominationValue: ko.Observable<string>;
    public denominationTotalValue: ko.Observable<string>;

    public resolve: ContosoDenominationInputDialogResolveFunction;

    constructor() {
        super();
        this.numpadValue = ko.observable('');
        this.currency = ko.observable('USD');
        this.denominationLabel = ko.observable('1.00');
        this.denominationValue = ko.observable('2.00');
        this.denominationTotalValue = ko.observable('4.00');

        this._numPadOptions = {
            decimalPrecision: 0,
            globalInputBroker: undefined,
            label: "Enter Quantity",
            value: 0
        };
    }


    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        let numPadRootElem: HTMLDivElement = element.querySelector("#NumericNumPad") as HTMLDivElement;
        this.numPad = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "NumericNumPad", this._numPadOptions, numPadRootElem);
        this.numPad.addEventListener("EnterPressed", (eventData: { value: Commerce.Extensibility.NumPadValue }): Promise<IContosoDenominationInputDialogResult> => {
            return this._resolvePromise({ canceled: false, item: this._item, value: Number(eventData.value.toString())});
        });

        this.numPad.addEventListener("ValueChanged", (eventData: { value: Commerce.Extensibility.NumPadValue }) => {
            // Update UI
            let denominationQty: number = Number(eventData.value.toString());
            this.denominationValue(CurrencyFormatter.toCurrency(denominationQty));
            let denominationAmount: number = ContosoNumberExtensions.roundToNDigits(denominationQty * this._item.DenominationAmount, 2);
            this.denominationTotalValue(CurrencyFormatter.toCurrency(denominationAmount));

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

    private onNumPadEnter(value: Commerce.Extensibility.NumPadValue) {
        this.numpadValue(value.toString());
        this.numPad.value = 0;
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