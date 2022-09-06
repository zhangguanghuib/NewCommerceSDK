
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ArrayExtensions, ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";

import {
    FulfillmentLineExtensionCommandBase,
    FulfillmentLinesSelectedData,
    FulfillmentLinePackingSlipSelectedData,
    FulfillmentLinesLoadedData,
    IFulfillmentLineExtensionCommandState,
    IExtensionCommandToFulfillmentLineMessageTypeMap,
    IFulfillmentLineToExtensionCommandMessageTypeMap
} from "PosApi/Extend/Views/FulfillmentLineView";

import { ShowMessageDialogClientRequest, ShowMessageDialogClientResponse, IMessageDialogOptions } from "PosApi/Consume/Dialogs";

import { GetScanResultClientRequest, GetScanResultClientResponse } from "PosApi/Consume/ScanResults";

import BarcodeMsrDialog from "./BarcodeMsrDialog";

export default class FulfillmentLineCommand extends FulfillmentLineExtensionCommandBase {

    private _fulfillmentLines: ProxyEntities.FulfillmentLine[];
    private _selectedFulfillmentLines: ProxyEntities.FulfillmentLine[];

    constructor(context: IExtensionCommandContext<IFulfillmentLineToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "sampleFufillmentLineCommand";
        this.label = "Scan and Select Product";
        this.extraClass = "iconLightningBolt";

        this._selectedFulfillmentLines = [];
        this.fulfillmentLinesSelectionHandler = (data: FulfillmentLinesSelectedData): void => {
            this._selectedFulfillmentLines = data.fulfillmentLines;
        }

        this.fulfillmentLinesSelectionClearedHandler = (): void => {
            this._selectedFulfillmentLines = [];
        }

        this.packingSlipSelectedHandler = (data: FulfillmentLinePackingSlipSelectedData): void => {
            this.isVisible = false;
        }

        this.packingSlipSelectionClearedHandler = (): void => {
            this.isVisible = false;
        }

        this.fulfillmentLinesLoadedHandler = (data: FulfillmentLinesLoadedData): void => {
            this._fulfillmentLines = data.fulfillmentLines;
        }
    }

    protected init(state: IFulfillmentLineExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }


    protected execute(): void {
        throw new Error("Method not implemented.");
    }

}