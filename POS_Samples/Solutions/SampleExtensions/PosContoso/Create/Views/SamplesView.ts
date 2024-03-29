﻿import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import ko from "knockout";

export interface ISampleItem {
    label: string;
    viewName?: string;
    items?: ISampleItem[];
}

export default class SamplesView extends Views.CustomViewControllerBase {
    samplesList: { label: string; items: { label: string; viewName: string; }[]; }[];

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);

        this.state.title = "Samples";

        this.samplesList = [
            {
                label: "Pos Controls",
                items: [
                    { label: "AlphanumericNumPad", viewName: "AlphanumericNumPadView" },
                    { label: "AppBar", viewName: "AppBarView" },
                    { label: "CurrencyNumPad", viewName: "CurrencyNumPadView" },
                    { label: "DataList", viewName: "DataListView" },
                    { label: "DataList (dynamic)", viewName: "DynamicDataListView" },
                    { label: "DatePicker", viewName: "DatePickerView" },
                    { label: "Loader", viewName: "LoaderView" },
                    { label: "Menu", viewName: "MenuView" },
                    { label: "NumericNumPad", viewName: "NumericNumPadView" },
                    { label: "TimePicker", viewName: "TimePickerView" },
                    { label: "ToggleMenu", viewName: "ToggleMenuView" },
                    { label: "ToggleSwitch", viewName: "ToggleSwitchView" },
                    { label: "TransactionNumPad", viewName: "TransactionNumPadView" },
                ]
            },
            {
                label: "Api",
                items: [
                    { label: "ApiView", viewName: "ApiView" },
                    { label: "AddTenderLineToCart", viewName: "AddTenderLineToCartView" },
                    { label: "CloseShift", viewName: "CloseShiftView" },
                    { label: "ForceVoidTransactionView", viewName: "ForceVoidTransactionView" },
                    { label: "SyncStockCountJournalsView", viewName: "SyncStockCountJournalsView" },
                    { label: "VoidTenderLineView", viewName: "VoidTenderLineView" },
                    { label: "VoidCartLineView", viewName: "VoidCartLineView" }
                ]
            },
            {
                label: "Other",
                items: [
                    { label: "SimpleExtensionView", viewName: "SimpleExtensionView" }
                ]
            },
            {
                label: "Dialogs",
                items: [
                    { label: "ListInputDialogView", viewName: "ListInputDialogView" },
                    { label: "AlphanumericInputDialogView", viewName: "AlphanumericInputDialogView" },
                    { label: "NumericInputDialogView", viewName: "NumericInputDialogView" },
                    { label: "TextInputDialogView", viewName: "TextInputDialogView" }
                ]
            }
        ];
    }

    /**
         * Bind the html element with view controller.
         * @param {HTMLElement} element DOM element.
         */
    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    /**
     * Click handler for sample link.
     * @param sampleItem Sample item that was clicked.
     */
    public sampleClick(sampleItem: ISampleItem): void {
        this.context.navigator.navigate(sampleItem.viewName);
    }
}