/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import * as Views from "PosApi/Create/Views";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { StoreOperations } from "../../../DataService/DataServiceRequests.g";
import ko from "knockout";

/**
 * The controller for VoidCartLineView.
 */
export default class VoidCartLineView extends Views.CustomViewControllerBase {
    public qrText: ko.Observable<string>;
    public _selectedJournal: ProxyEntities.SalesOrder;
    public imageBase64: ko.Observable<string>;

    /**
     * Creates a new instance of the VoidCartLineView class.
     * @param {Views.ICustomViewControllerContext} context The custom view controller context.
     * @param {any} [options] The options used to initialize the view state.
     */
    constructor(context: Views.ICustomViewControllerContext, options?: any) {
        // Do not save in history
        super(context);
        this.state.title = "Receipt BarCode";
        this._selectedJournal = options;
        this.qrText = ko.observable(this._selectedJournal.ReceiptId);
        this.imageBase64 = ko.observable("");
    }

    /**
     * Bind the html element with view controller.
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);
        this.getBarcode();
    }

    public getBarcode(): void {
        this.state.isProcessing = true;

        let GetBarcodeRequest: StoreOperations.GetBarcodeBase64StringRequest<StoreOperations.GetBarcodeBase64StringResponse>
            = new StoreOperations.GetBarcodeBase64StringRequest(this._selectedJournal.ReceiptId);

        this.context.runtime.executeAsync(GetBarcodeRequest)
            .then((result: ClientEntities.ICancelableDataResult<StoreOperations.GetBarcodeBase64StringResponse>): Promise<void> => {
                if (!result.canceled) {
                    this.imageBase64("data:image/png;base64," + result.data.result);
                }
                this.state.isProcessing = false;
                return Promise.resolve();
            }).catch((reason: any) => {
                this.state.isProcessing = false;
            });
    }
    
    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}