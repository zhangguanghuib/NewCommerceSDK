import {
    ShowTextInputDialogClientRequest, ShowTextInputDialogClientResponse, ITextInputDialogOptions,
    ShowTextInputDialogError, ITextInputDialogResult
} from "PosApi/Consume/Dialogs";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";

export default class TextInputDialog {
    public show(context: IExtensionContext, message: string): Promise<string> {
        let promise: Promise<string> = new Promise<string>((resolve: (num: string) => void, reject: (reason?: any) => void) => {

            let textInputDialogOptions: ITextInputDialogOptions = {
                title: "", 
                subTitle: "", 
                label: "Input",
                defaultText: "Input your questions here",
                textInputType:0,
                onBeforeClose: this.onBeforeClose.bind(this)
            };

            let dialogRequest: ShowTextInputDialogClientRequest<ShowTextInputDialogClientResponse> =
                new ShowTextInputDialogClientRequest<ShowTextInputDialogClientResponse>(textInputDialogOptions);

            context.runtime.executeAsync(dialogRequest)
                .then((result: ClientEntities.ICancelableDataResult<ShowTextInputDialogClientResponse>) => {
                    if (!result.canceled) {
                        context.logger.logInformational("Text Entered in Box: " + result.data.result.value);
                        resolve(result.data.result.value);
                    } else {
                        context.logger.logInformational("Text Dialog is canceled.");
                        resolve(null);
                    }
                }).catch((reason: any) => {
                    context.logger.logError(JSON.stringify(reason));
                    reject(reason);
                });
        });

        return promise;
    }
    /**
     * Decides what to do with the dialog based on the button pressed and input
     * @param {ClientEntities.ICancelableDataResult<ITextInputDialogResult>} result Input result of dialog
     * @param {Promise<void>} The returned promise
     */
    private onBeforeClose(result: ClientEntities.ICancelableDataResult<ITextInputDialogResult>): Promise<void> {
        if (!result.canceled) {
            if (!ObjectExtensions.isNullOrUndefined(result.data)) {
                return Promise.resolve();     
            } else {
                // Should not reach this branch
                let error: ShowTextInputDialogError = new ShowTextInputDialogError("Data result is null.");
                return Promise.reject(error);
            }
        } else {
            // Note that if result.cancelled is true, then result.data is null
            let error: ShowTextInputDialogError = new ShowTextInputDialogError("Cannot close dialog. Must enter value");
            return Promise.reject(error);
        }
    }
}