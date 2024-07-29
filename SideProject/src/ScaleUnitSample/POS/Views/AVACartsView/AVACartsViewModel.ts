/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { IExtensionViewControllerContext } from "PosApi/Create/Views";
//import { Entities } from "../DataService/DataServiceEntities.g";
//import * as Messages from "../DataService/DataServiceRequests.g";
//import PingResultDialog from "../Controls/Dialogs/Display/PingResultDialogModule";
//import { ObjectExtensions, ArrayExtensions } from "PosApi/TypeExtensions";

/**
 * The ViewModel for AVACartsView.
 */
export default class AVACartsViewModel {
    public title: string;
    //public loadedData: Entities.AVACartsEntity[];
    public isItemSelected: () => boolean;
    /*    private _selectedItem: Entities.AVACartsEntity;*/
    public _context: IExtensionViewControllerContext;

    constructor(context: IExtensionViewControllerContext) {
        this._context = context;
        this.title = context.resources.getString("string_0001");
        //this.loadedData = [];
        //this.isItemSelected = () => !ObjectExtensions.isNullOrUndefined(this._selectedItem);
    }

    public load(): Promise<void> {
        //return this._context.runtime
        //    .executeAsync(new Messages.BoundController.GetAllAVACartsEntitiesRequest())
        //    .then(response => {
        //        if (!response.canceled) {
        //            this.loadedData = response.data.result;
        //        }
        //    });
        return Promise.resolve();
    }

    /**
     * Handler for list item selection.
     * @param {Entities.AVACartsEntity[]} items
     */
    //public seletionChanged(items: Entities.AVACartsEntity[]): Promise<void> {
    //    this._context.logger.logInformational("Item selected:" + JSON.stringify(items));
    //    this._selectedItem = ArrayExtensions.firstOrUndefined(items);
    //    return Promise.resolve();
    //}
    
    public deleteAVACartsEntity(): Promise<void> {
        // Delete the selected entity and reload the loaded data to reflect the change:
        //return this._context.runtime
        //    .executeAsync(new Messages.BoundController.DeleteAVACartsEntityRequest(this._selectedItem.UnusualEntityId))
        //    .then(response => {
        //        if (!response.canceled && response.data.result) {
        //            this._context.logger.logInformational("Delete success for id: " + this._selectedItem.UnusualEntityId);
        //            return this.load(); // Load the updated data
        //        }
        //        this._context.logger.logInformational("Delete failed for id " + this._selectedItem.UnusualEntityId);
        //        return Promise.resolve();
        //    });
        return Promise.resolve();
    }

    public runPingTest(): Promise<void> {
        //return this._context.runtime
        //    .executeAsync(new Messages.StoreOperations.SimplePingGetRequest())
        //    .then(pingGetResponse => {
        //        return this._context.runtime
        //            .executeAsync(new Messages.StoreOperations.SimplePingPostRequest())
        //            .then(pingPostResponse => {
        //                let pingResultDialog: PingResultDialog = new PingResultDialog();
        //                return pingResultDialog.open(pingGetResponse.data.result, pingPostResponse.data.result);
        //            });
        //    });

        return Promise.resolve();
    }
}
