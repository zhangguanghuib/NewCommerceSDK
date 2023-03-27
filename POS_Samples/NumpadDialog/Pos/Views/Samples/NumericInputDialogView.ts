import ko from "knockout";
import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";

import NumericInputDialog from "../../Controls/DialogSample/NumericInputDialog";

/**
 * The controller for NumericInputDialogView.
 */
export default class NumericInputDialogView extends Views.CustomViewControllerBase {
    public dialogResult: ko.Observable<string>;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);

        this.state.title = "NumericInputDialog sample";
        this.dialogResult = ko.observable("");
    }

    /**
     * Bind the html element with view controller.
     *
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    /**
     * Opens the numeric input dialog sample.
     */
    public openNumericInputDialog(): void {
        let numericInputDialog: NumericInputDialog = new NumericInputDialog();
        numericInputDialog.show(this.context, "Hello World")
            .then((result: string) => {
                this.dialogResult(result);
            }).catch((reason: any) => {
                this.context.logger.logError("NumericInputDialog: " + JSON.stringify(reason));
            });
    }

    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}
