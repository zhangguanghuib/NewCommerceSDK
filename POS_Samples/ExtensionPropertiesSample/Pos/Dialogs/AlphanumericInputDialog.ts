import { IAlphanumericInputDialogOptions, IAlphanumericInputDialogResult, ShowAlphanumericInputDialogClientRequest, ShowAlphanumericInputDialogClientResponse, ShowAlphanumericInputDialogError } from "PosApi/Consume/Dialogs";
import { ClientEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class AlphanumericInputDialog {

    public show(context: IExtensionContext, message: string): Promise<string> {

        let promise: Promise<string> = new Promise<string>(
            (resolve: (num: string) => void, reject: (reason?: any) => void) => {
                let subTitleMsg: string = "Input the coupon code";
                let alphanumericInputDialogOptions: IAlphanumericInputDialogOptions = {
                    title: "Voice Call",
                    subTitle: subTitleMsg,
                    numPadLabel: "Please input Coupon Code",
                    defaultValue: "abc123",
                    onBeforeClose: this.onBeforeClose.bind(this)
                };

                let dialogRequest: ShowAlphanumericInputDialogClientRequest<ShowAlphanumericInputDialogClientResponse> =
                    new ShowAlphanumericInputDialogClientRequest<ShowAlphanumericInputDialogClientResponse>(alphanumericInputDialogOptions);

                context.runtime.executeAsync(dialogRequest)
                    .then((result: ClientEntities.ICancelableDataResult<ShowAlphanumericInputDialogClientResponse>) => {
                        if (!result.canceled) {
                            context.logger.logInformational("AlphanumericInputDialog result: " + result.data.result.value);
                            resolve(result.data.result.value);
                        } else {
                            context.logger.logInformational("AlphanumericInputDialog is cancelled");
                            resolve(null);
                        }
                    }).catch((reason: any) => {
                        context.logger.logError(JSON.stringify(reason));
                        reject(reason);
                    });
            }
        );

        return promise;
    }

    private onBeforeClose(result: ClientEntities.ICancelableDataResult<IAlphanumericInputDialogResult>): Promise<void> {
        if (!result.canceled) {
            if (!ObjectExtensions.isNullOrUndefined(result.data)) {
                if (result.data.value === "111") {
                    let error: ShowAlphanumericInputDialogError =
                        new ShowAlphanumericInputDialogError("Invalid input. Enter different value.", "2121" /* new default value */);
                    return Promise.reject(error);
                } else {
                    return Promise.resolve();
                }
            } else {
                // Should not reach this branch
                let error: ShowAlphanumericInputDialogError = new ShowAlphanumericInputDialogError("Data result is null.");
                return Promise.reject(error);
            }
        } else {
            // Note that if result.cancelled is true, then result.data is null
            let error: ShowAlphanumericInputDialogError =
                new ShowAlphanumericInputDialogError("Cannot close dialog. Must enter value");
            return Promise.reject(error);
        }
    }
}