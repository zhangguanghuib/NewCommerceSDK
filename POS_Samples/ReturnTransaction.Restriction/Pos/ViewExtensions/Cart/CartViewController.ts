import { ProxyEntities } from "PosApi/Entities";
import * as CartView from "PosApi/Extend/Views/CartView";
import { ArrayExtensions, StringExtensions } from "PosApi/TypeExtensions";
export default class CartViewController extends CartView.CartExtensionViewControllerBase {
    public static selectedCartLineId: string = StringExtensions.EMPTY;
    private _selectedCartLines: ProxyEntities.CartLine[];
    public _selectedTenderLines: ProxyEntities.TenderLine[];
    public _isProcessingAddItemOrCustomer: boolean;

    constructor(context: CartView.IExtensionCartViewControllerContext) {
        super(context);

        this.cartLineSelectedHandler = (data: CartView.CartLineSelectedData): void => {
            this._selectedCartLines = data.cartLines;
            if (ArrayExtensions.hasElements(this._selectedCartLines)) {
                CartViewController.selectedCartLineId = this._selectedCartLines[0].LineId;
            }
        }

        this.cartLineSelectionClearedHandler = (): void => {
            this._selectedCartLines = [];
            CartViewController.selectedCartLineId = null;
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