import { IMessageDialogOptions, ShowMessageDialogClientRequest, ShowMessageDialogClientResponse } from "PosApi/Consume/Dialogs";
import { ClientEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";

export default class MessageDialog {
    public static show(context: IExtensionContext, title: string, message: string): Promise<void> {
        let messageDialogOptions: IMessageDialogOptions = {
            title: title,
            message: message,
            showCloseX: true,
            button1: {
                id: "Button1Close",
                label: context.resources.getString("string_50"),
                result: "OKResult"
            }
        };

        let dialogRequest: ShowMessageDialogClientRequest<ShowMessageDialogClientResponse> =
            new ShowMessageDialogClientRequest(messageDialogOptions);

        return context.runtime.executeAsync(dialogRequest)
            .then((value: ClientEntities.ICancelableDataResult<ShowMessageDialogClientResponse>) => {
                return Promise.resolve();
            });
    }
}