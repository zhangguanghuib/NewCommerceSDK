import { INumericInputDialogOptions, INumericInputDialogResult, ShowNumericInputDialogClientRequest, ShowNumericInputDialogClientResponse, ShowNumericInputDialogError } from "PosApi/Consume/Dialogs";
import { ClientEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class NumericInputDialog {

    public show(context: IExtensionContext, message: string) {
        let promise: Promise<string> = new Promise<string>((resolve: (num: string) => void, reject: (reason?: any) => void) => {
            let subTitleMsg: string = "Enter voice call verification code or void transaction.\n\n"
                + "The merchant is responsible for entering a valid Auth Code.\n\n"
                + "If an invalid auth code is sent to the bank, the batch cannot be settled and the merchant may get fined ($50 for each instance currently).";

            let numericInputDialogOptions: INumericInputDialogOptions = {
                title: "Voice Call",
                subTitle: subTitleMsg,
                numPadLabel: "Please enter code",
                defaultNumber: "0000",
                onBeforeClose: this.onBeforeClose.bind(this)
            };

            let dialogRequest: ShowNumericInputDialogClientRequest<ShowNumericInputDialogClientResponse> =
                new ShowNumericInputDialogClientRequest<ShowNumericInputDialogClientResponse>(numericInputDialogOptions);

            context.runtime.executeAsync(dialogRequest)
                .then((result: ClientEntities.ICancelableDataResult<ShowNumericInputDialogClientResponse>) => {
                    if (!result.canceled) {
                        resolve(result.data.result.value);
                    } else {
                        resolve(null);
                    }
                }).catch((reason: any) => {
                    reject(reason);
                });
        });

        return promise;
    }

    public onBeforeClose(result: ClientEntities.ICancelableDataResult<INumericInputDialogResult>): Promise<void> {
        if (!result.canceled) {
            if (!ObjectExtensions.isNullOrUndefined(result.data)) {
                if (result.data.value === "111") {
                    let error: ShowNumericInputDialogError = new ShowNumericInputDialogError(
                        "Invalid input. Enter different value.", "2121");
                    return Promise.reject(error);
                } else {
                    return Promise.resolve();
                }
            } else {
                let error: ShowNumericInputDialogError = new ShowNumericInputDialogError(
                    "Data result is null");
                return Promise.reject(error);
            }
        } else {
            let error: ShowNumericInputDialogError = new ShowNumericInputDialogError("Cannot close dialog. Must enter value");
            return Promise.reject(error);
        }
    }

}