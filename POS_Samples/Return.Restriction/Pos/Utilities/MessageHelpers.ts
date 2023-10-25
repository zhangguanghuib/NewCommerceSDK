import MessageDialog from "../Dialogs/MessageDialog";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";

export default class MessageHelpers {
    /**
     * Shows the message dialog.
     * @param {IExtensionContext} context The runtime context within the message is shown
     * @param {string} title The title to display
     * @param {string} message The message to display
     * @returns Promise<void> The promise (always success) after the message has been shown
     */
    public static ShowMessage(context: IExtensionContext, title: string, message: string): Promise<string> {
        return MessageDialog.show(context, message, title);
    }

    /**
     * Shows the error message dialog.
     * @param {IExtensionContext} context The runtime context within the message is shown
     * @param {string} message The message to display
     * @param {string} error The error object
     * @returns Promise<void> The promise after the message has been shown as success
     */
    public static ShowErrorMessage(context: IExtensionContext, message: string, error: any): Promise<void> {
        let title: string = context.resources.getString("string_70"); // Error:\n
        return MessageDialog.show(context, title, message).then(() => {
            return Promise.reject(error);
        }).catch(() => {
            return Promise.reject(error);
        });
    }
}