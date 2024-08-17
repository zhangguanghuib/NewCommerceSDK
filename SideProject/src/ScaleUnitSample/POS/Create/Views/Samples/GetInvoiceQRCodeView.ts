import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { Entities } from "../../../DataService/DataServiceEntities.g";
import ko from "knockout";
import { StoreOperations } from "../../../DataService/DataServiceRequests.g";

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

        this._selectedJournal = options;
        this.state.title = "Get Invoice QR Code";
        this.qrText = ko.observable("");
        this.imageBase64 = ko.observable("");
    }

    /**
     * Bind the html element with view controller.
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {

        this.qrText(`Scan the below QR Code to get the invoice Receipt ${this._selectedJournal.ReceiptId}`);

        let qrCodeUrl: string = this.getInvoiceQRCode(this._selectedJournal);

        let GetQRCodeRequest: StoreOperations.GetQRCodeBase64StringRequest<StoreOperations.GetQRCodeBase64StringResponse>
            = new StoreOperations.GetQRCodeBase64StringRequest(qrCodeUrl);

        this.context.runtime.executeAsync(GetQRCodeRequest)
            .then((result: ClientEntities.ICancelableDataResult<StoreOperations.GetQRCodeBase64StringResponse>): Promise<void> => {
                if (!result.canceled) {
                    this.imageBase64("data:image/png;base64," + result.data.result);
                }
                return Promise.resolve();
            });

        //this.qrText(this.getInvoiceQRCode(this._selectedJournal));
        ko.applyBindings(this, element);
    }

    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }


    private getInvoiceQRCode(row: ProxyEntities.SalesOrder): string {
        let cps: Array<ProxyEntities.CommerceProperty>
            = row.ExtensionProperties.filter((cp) => cp.Key === "QRCode");

        if (cps.length >= 1) {
            let cp: ProxyEntities.CommerceProperty = cps[0];
            return cp.Value.StringValue;
        }
        return "";
    }
}