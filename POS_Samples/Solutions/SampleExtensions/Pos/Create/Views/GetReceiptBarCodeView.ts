import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { StoreOperations } from "../../DataService/DataServiceRequests.g";
import ko from "knockout";

export default class GetReceiptBarCodeView extends Views.CustomViewControllerBase {
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

        this._selectedJournal = options;
        this.state.title = "Get Receipt Barcode";
        this.qrText = ko.observable("");
        this.imageBase64 = ko.observable("");
    }

    /**
     * Bind the html element with view controller.
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {

        ko.applyBindings(this, element);
    }

    public onShown() {

        this.state.isProcessing = true;
        this.qrText(`ReceiptId: ${this._selectedJournal.ReceiptId}`);

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