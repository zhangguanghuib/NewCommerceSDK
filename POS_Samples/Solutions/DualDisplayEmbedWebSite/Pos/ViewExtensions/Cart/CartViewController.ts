import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as CartView from "PosApi/Extend/Views/CartView";
import { ArrayExtensions, StringExtensions } from "PosApi/TypeExtensions";
import * as Messages from "../../DataService/DataServiceRequests.g";

export default class CartViewController extends CartView.CartExtensionViewControllerBase {
    public static selectedCartLineId: string = StringExtensions.EMPTY;
    public static selectedCartLines: ProxyEntities.CartLine[];
    public _selectedTenderLines: ProxyEntities.TenderLine[];
    public _isProcessingAddItemOrCustomer: boolean;

    constructor(context: CartView.IExtensionCartViewControllerContext) {
        super(context);
        CartViewController.selectedCartLines = [];
        CartViewController.selectedCartLineId = null;
        this.cartLineSelectedHandler = (data: CartView.CartLineSelectedData): void => {
            CartViewController.selectedCartLines = data.cartLines;
            if (ArrayExtensions.hasElements(CartViewController.selectedCartLines)) {
                CartViewController.selectedCartLineId = CartViewController.selectedCartLines[0].LineId;
                localStorage.setItem("currentCartLineId", CartViewController.selectedCartLineId);
            }
        }

        this.cartLineSelectionClearedHandler = (): void => {
            CartViewController.selectedCartLines = [];
            CartViewController.selectedCartLineId = null;
            localStorage.setItem("currentCartLineId", StringExtensions.EMPTY);
        }

        this.tenderLineSelectedHandler = (data: CartView.TenderLineSelectedData): void => {
            this._selectedTenderLines = data.tenderLines;
        }

        this.tenderLineSelectionClearedHandler = (): void => {
            this._selectedTenderLines = [];
        }

        this.processingAddItemOrCustomerChangedHandler = (processing: boolean): void => {
            this._isProcessingAddItemOrCustomer = processing;
        }

        this.cartChangedHandler = (data: CartView.CartChangedData): void => {
            console.log(data);
        }
    }

    protected init(state): void {
        console.log("Init is called");
        setTimeout(() => {
            let request: Messages.StoreOperations.GetHtmlContentRequest<Messages.StoreOperations.GetHtmlContentResponse>
                = new Messages.StoreOperations.GetHtmlContentRequest<Messages.StoreOperations.GetHtmlContentResponse>("https://www.lifepharmacy.com/");

            this.context.runtime.executeAsync(request).then((result: ClientEntities.ICancelableDataResult<Messages.StoreOperations.GetHtmlContentResponse>): Promise<string> => {
                if (result.canceled) {
                    return Promise.reject("Request Cancelled");
                } else {
                    localStorage.setItem("DualDisPlayWebSiteContent", result.data.result);
                    return Promise.resolve(result.data.result);
                }
            });
        }, 5000);
    }
}