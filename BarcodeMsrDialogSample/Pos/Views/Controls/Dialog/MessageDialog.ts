

import { ShowMessageDialogClientRequest, ShowMessageDialogClientResponse, IMessageDialogOptions } from "PosApi/Consume/Dialogs";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ClientEntities } from "PosApi/Entities";

export default class MessageDialog {
    public static show(context: IExtensionContext, operationName: string, showTitle: boolean, showOkButton: boolean, showCancelButton: boolean, message: string): Promise<void> {
        let promise: Promise<void> = new Promise<void>((resolve: () => void, reject: (reason?: any) => void) => {
            let messageDialogOptions: IMessageDialogOptions = {                
                message: message,
                showCloseX: false, // this property will return "Close" as result when "X" is clicked to close dialog.                
            };

            if (showTitle){
                messageDialogOptions.title = operationName;
            }

            if (showOkButton) {
                messageDialogOptions.button1 = {
                    id: "ButtonOk",
                    label: "Ok",
                    result: "OKResult"
                }
            }

            if (showCancelButton){
                messageDialogOptions.button2 = {
                    id: "ButtonCancel",
                    label: "Cancel",
                    result: "CancelResult"
                }
            }

            let dialogRequest: ShowMessageDialogClientRequest<ShowMessageDialogClientResponse> =
                new ShowMessageDialogClientRequest<ShowMessageDialogClientResponse>(messageDialogOptions);

            context.runtime.executeAsync(dialogRequest).then((result: ClientEntities.ICancelableDataResult<ShowMessageDialogClientResponse>) => {
                if (!result.canceled) {
                    context.logger.logInformational("MessageDialog result: " + result.data.result.dialogResult);
                    resolve();
                }
            }).catch((reason: any) => {
                context.logger.logError(JSON.stringify(reason));
                reject(reason);
            });
        });

        return promise;
    }
}