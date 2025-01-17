import * as InventoryAdjustmentDocumentWorkingView from "PosApi/Extend/Views/InventoryAdjustmentDocumentWorkingView";
import { ProxyEntities } from "PosApi/Entities";
import {
    IInventoryAdjustmentDocumentWorkingExtensionCommandContext,
    InventoryAdjustmentDocumentWorkingDocumentUpdatedData,
    InventoryAdjustmentDocumentWorkingLinesLoadedData,
    InventoryAdjustmentDocumentWorkingLinesSelectedData,
    InventoryAdjustmentDocumentWorkingLineUpdatedData,
    InventoryAdjustmentDocumentWorkingReceiptSelectedData
} from "PosApi/Extend/Views/InventoryAdjustmentDocumentWorkingView";
export default class InventoryAdjustmentCommandTest01 extends InventoryAdjustmentDocumentWorkingView.InventoryAdjustmentDocumentWorkingExtensionCommandBase {

    protected readonly context: IInventoryAdjustmentDocumentWorkingExtensionCommandContext;
    protected document: Readonly<ProxyEntities.InventoryInboundOutboundDocument>;
    constructor(context: IInventoryAdjustmentDocumentWorkingExtensionCommandContext) {
        super(context);

        this.id = "InventoryAdjustmentDocumentWorkingExtensionCommandTest01";
        this.label = "Test 01";
        this.extraClass = "iconSetAction";
        this.isVisible = true;
        this.canExecute = true;

        this.documentUpdatedHandler = (data: InventoryAdjustmentDocumentWorkingDocumentUpdatedData): void => {
            console.log(data);
        };

        this.documentLinesSelectedHandler = (data: InventoryAdjustmentDocumentWorkingLinesSelectedData): void => {
            console.log(data);
        };

        this.documentLinesSelectionClearedHandler = () => {
            console.log("documentLinesSelectionClearedHandler");
        };

        this.receiptsSelectedHandler = (data: InventoryAdjustmentDocumentWorkingReceiptSelectedData): void => {
            console.log(data);
        };

        this.receiptsSelectionClearedHandler = () => {
            console.log("receiptsSelectionClearedHandler")
        };

        this.documentLinesLoadedHandler = (data: InventoryAdjustmentDocumentWorkingLinesLoadedData): void => {
            console.log(data)
        };

        this.documentLineUpdatedHandler = (data: InventoryAdjustmentDocumentWorkingLineUpdatedData): void => {
            console.log(data)
        };

    }

    protected init(state: InventoryAdjustmentDocumentWorkingView.IInventoryAdjustmentDocumentWorkingExtensionCommandState): void {
        console.log("InventoryAdjustmentDocumentWorkingView->Init.");
        this.document = state.document;   
    }

    protected execute(): void {
        console.log("InventoryAdjustmentDocumentWorkingView->Execute.");
    }
}