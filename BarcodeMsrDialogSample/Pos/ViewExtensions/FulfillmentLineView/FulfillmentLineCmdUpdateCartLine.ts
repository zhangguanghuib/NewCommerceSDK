import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import {
    FulfillmentLineExtensionCommandBase,
    FulfillmentLinePackingSlipSelectedData,
    FulfillmentLinesLoadedData,
    FulfillmentLinesSelectedData,
    IFulfillmentLineToExtensionCommandMessageTypeMap
} from "PosApi/Extend/Views/FulfillmentLineView";

export default class FulfillmentLineCmdUpdateCartLine extends FulfillmentLineExtensionCommandBase {

    constructor(context: IExtensionCommandContext<IFulfillmentLineToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "FulfillmentLineCmdUpdateCartLine";
        this.label = "Update Cart Line";
        this.extraClass = "iconBuy";

        this.fulfillmentLinesSelectionHandler = (data: FulfillmentLinesSelectedData): void => {
        }

        this.fulfillmentLinesSelectionClearedHandler = (): void => {
        }

        this.packingSlipSelectedHandler = (data: FulfillmentLinePackingSlipSelectedData): void => {
            this.isVisible = false;
        }

        this.packingSlipSelectionClearedHandler = (): void => {
            this.isVisible = false;
        }

        this.fulfillmentLinesLoadedHandler = (data: FulfillmentLinesLoadedData): void => {
        }
    }

    protected init(state: Commerce.Extensibility.IFulfillmentLineExtensionCommandState): void {
        throw new Error("Method not implemented.");
    }


    protected execute(): void {
        throw new Error("Method not implemented.");
    }
}