import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import * as InventoryAdjustmentDocumentWorkingView from "PosApi/Extend/Views/InventoryAdjustmentDocumentWorkingView";

export default class EccoInventoryAdjustmentShowTotalCommand extends InventoryAdjustmentDocumentWorkingView.InventoryAdjustmentDocumentWorkingExtensionCommandBase {

    private isCommandEnabled: boolean = false;

    constructor(context: IExtensionCommandContext<InventoryAdjustmentDocumentWorkingView.IInventoryAdjustmentDocumentWorkingToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "EccoShowTotalInvAdjDocument";
        this.label = "Total qty.";
        this.extraClass = "iconCalculate";

        this.documentLinesLoadedHandler = (data: InventoryAdjustmentDocumentWorkingView.InventoryAdjustmentDocumentWorkingLinesLoadedData): void => {
            if (this.isCommandEnabled) {
                this.canExecute = true;
            }
        };
    }

    protected init(state: InventoryAdjustmentDocumentWorkingView.IInventoryAdjustmentDocumentWorkingExtensionCommandState): void {

        this.isCommandEnabled = true;
        this.isVisible = (this.isCommandEnabled
            && (!ObjectExtensions.isNullOrUndefined(state.document.SourceDocument))
            && (state.document.SourceDocument.StatusValue === ProxyEntities.InventoryInboundOutboundSourceDocumentStatus.Posted
                || state.document.SourceDocument.StatusValue === ProxyEntities.InventoryInboundOutboundSourceDocumentStatus.Committed));

    }

    protected execute(): void {
        this.context.logger.logInformational("Executing EccoInventoryAdjustmentShowTotalCommand.execute()");
    }

}