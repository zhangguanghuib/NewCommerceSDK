import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import {
    IInventoryLookupMatrixExtensionMenuCommandContext,
    InventoryLookupMatrixExtensionMenuCommandBase,
    InventoryLookupMatrixItemAvailabilitySelectedData,
    InventoryLookupMatrixStoreChangedData,
    IInventoryLookupMatrixExtensionMenuCommandState
} from "PosApi/Extend/Views/InventoryLookupMatrixView";

import { IMessageDialogOptions, ShowMessageDialogClientRequest, ShowMessageDialogClientResponse } from "PosApi/Consume/Dialogs";

export default class MoreDetailsMenuCommand extends InventoryLookupMatrixExtensionMenuCommandBase {
    public readonly label;
    public readonly id;

    public _selectedAvailability: ProxyEntities.ItemAvailability;
    public _store: ProxyEntities.OrgUnit;

    constructor(context: IInventoryLookupMatrixExtensionMenuCommandContext) {
        super(context);

        this.label = "More details";
        this.id = "moreDetailsMenuCommand";
        this.itemAvailabilitySelectedHandler = (data: InventoryLookupMatrixItemAvailabilitySelectedData): void => {
            this._selectedAvailability = data.itemAvailability;
            this.canExecute = !this.canExecute;
        };

        this.storeChangedHandler = (data: InventoryLookupMatrixStoreChangedData): void => {
            this._store = data.store;
        }
    }

    protected init(state: IInventoryLookupMatrixExtensionMenuCommandState) {
        this.context.logger.logInformational("MoreDetailsMenuCommand.init was called");
        this._store = state.store;
    }


    public execute(): void {
        const CORRELATION_ID: string = this.context.logger.getNewCorrelationId();
        this.context.logger.logInformational("MoreDetailsMenuCommand.execute was called.", CORRELATION_ID);

        this.isProcessing = true;

        let options: IMessageDialogOptions = {
            title: "More Details",
            subTitle: "Item availability details",
            message: JSON.stringify(this._selectedAvailability),
            button1: {
                id: "okButton",
                label: "OK",
                isPrimary: true,
                result: "OkButtonResult"
            }
        };

        let request: ShowMessageDialogClientRequest<ShowMessageDialogClientResponse> = new ShowMessageDialogClientRequest<ShowMessageDialogClientResponse>(options, CORRELATION_ID);
        this.context.runtime.executeAsync(request)
            .then((result: ClientEntities.ICancelableDataResult<ShowMessageDialogClientResponse>) : void=> {
                this.isProcessing = false;
                this.canExecute = false;
                this.context.logger.logInformational("MoreDetailsMenuCommand.execute show message dialog request completed successfully.", CORRELATION_ID);

            }).catch((reason: any) => {
                this.isProcessing = false;
                this.canExecute = false;
                this.context.logger.logError("MoreDetailsMenuCommand.execute show message dialog request failed.", reason, CORRELATION_ID);
            });

    }
 }