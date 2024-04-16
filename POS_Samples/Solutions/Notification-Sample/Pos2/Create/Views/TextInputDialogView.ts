import ko from "knockout";
import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";

import TextInputDialog from "../../Create/Dialogs/DialogSample/TextInputDialog";

/**
 * The controller for TextInputDialogView.
 */
export default class TextDialogView extends Views.CustomViewControllerBase {
    public dialogResult: ko.Observable<string>;

    constructor(context: Views.ICustomViewControllerContext) {
        super(context);

        this.state.title = "TextInputDialog sample";
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
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    /**
     * Opens the text dialog sample.
     */
    public openTextDialog(): void {
        let textInputDialog: TextInputDialog = new TextInputDialog();
        textInputDialog.show(this.context, this.context.resources.getString("string_55"))
            .then((result: string) => {
                this.dialogResult(result);
            }).catch((reason: any) => {
                this.context.logger.logError("TextInputDialog: " + JSON.stringify(reason));
            });
    }
}