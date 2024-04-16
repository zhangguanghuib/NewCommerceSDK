
import * as Views from "PosApi/Create/Views";

import ko from "knockout";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import NumericInputDialog from "../../Dialogs/DialogSample/NumericInputDialog";

export default class NumericInputDialogView extends Views.CustomViewControllerBase {

    public dialogResult: ko.Observable<string>;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);

        this.state.title = "NumericInputDialog sample";
        this.dialogResult = ko.observable("");
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
    }

    public openNumericInputDialog(): void {

        let numericInputDialog: NumericInputDialog = new NumericInputDialog();
        numericInputDialog.show(this.context, "Hello world")
            .then((result: string) => {
                this.dialogResult(result);
            }).catch((reason: any) => {
                this.context.logger.logError("NumericInputDialog: " + JSON.stringify(reason));
            })


    }

}