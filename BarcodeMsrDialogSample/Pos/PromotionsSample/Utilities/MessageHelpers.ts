import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import MessageDialog from "PromotionsSample/Controls/Dialogs/MessageDialog";

export default class MessageHelps {
    public static ShowMessage(context: IExtensionContext, title: string, message: string): Promise<void> {
        return MessageDialog.show(context, title, message);
    }

    public static ShowErrorMessage(context: IExtensionContext, message: string, error: any): Promise<void> {
        let title: string = context.resources.getString("string_70");
        return MessageDialog.show(context, title, message).then(() => {
            return Promise.reject(error);
        }).catch(() => {
            return Promise.reject(error);
        });
    }
}