import { ProxyEntities } from "PosApi/Entities";
import * as CartView from "PosApi/Extend/Views/CartView";
import { ArrayExtensions, StringExtensions } from "PosApi/TypeExtensions";

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
}