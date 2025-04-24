import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import *  as InventoryDocument from "PosApi/Extend/Views/InventoryDocumentShippingAndReceivingView";
import { ProxyEntities } from "PosApi/Entities";
import ImageCaptureDialogModule from "../../Controls/Dialogs/Create/ImageCapture/ImageCaptureDialogModule";
import { IImageCaptureDialogResult } from "../../Controls/Dialogs/Create/ImageCapture/IImageCaptureDialogResult";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class CaptureAttachPictureCommand extends InventoryDocument.InventoryDocumentShippingAndReceivingExtensionCommandBase {
    private SourceDocument: ProxyEntities.InventoryInboundOutboundSourceDocument;

    public constructor(context: IExtensionCommandContext<InventoryDocument.IInventoryDocumentShippingAndReceivingToExtensionCommandMessageTypeMap>) {
        super(context);

        this.label = " Capture";
        this.id = "CaptureAttachPictureCommand";
        this.extraClass = "iconMapPin";
    }

    protected init(state: InventoryDocument.IInventoryDocumentShippingAndReceivingExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
        this.SourceDocument = state.document.SourceDocument;
        console.log("ExportFullOrderListCommand - init");
    }

    protected execute(): void {
        this.openTemplatedDialog();
    }

    public openTemplatedDialog(): void {
        let dialog: ImageCaptureDialogModule = new ImageCaptureDialogModule();
        dialog.open("").then((result: IImageCaptureDialogResult) => {
            if (!ObjectExtensions.isNullOrUndefined(result)
                && !ObjectExtensions.isNullOrUndefined(result.imageData)) {
                let imageExtensionProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
                imageExtensionProperty.Key = "image";
                imageExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
                imageExtensionProperty.Value.StringValue = result.imageData;
                this.SourceDocument.ExtensionProperties.push(imageExtensionProperty);
            }
        }).catch((reason: any) => {
            this.context.logger.logError("ImageCaptureDialog: " + JSON.stringify(reason));
        });
    }

}
