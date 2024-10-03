import { ProxyEntities } from "PosApi/Entities";
import * as CartView from "PosApi/Extend/Views/CartView";
import { ArrayExtensions, StringExtensions } from "PosApi/TypeExtensions";
import CartViewUtils from "./CartViewUtils";

declare var Commerce: any;
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

            let viewName: string = Commerce.ViewModelAdapter.getCurrentViewName();
            console.log(viewName);
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
            // Total Items Div
            let totalItems: HTMLDivElement = document.querySelector('#TotalItemsField') as HTMLDivElement;
            if (totalItems) {
                CartViewUtils.setStyle(totalItems, {
                    'backgroundColor': 'lightblue',
                    'padding': '5px',
                    'color': 'black'
                });
                // Total Items Text
                let totalItemsTextDiv: HTMLDivElement = totalItems.firstElementChild.firstElementChild.firstElementChild as HTMLDivElement;
                CartViewUtils.setStyle(totalItemsTextDiv, {
                    'fontFamily': 'Arial, sans-serif',
                    'fontSize': '16px',
                    'fontWeight': 'bold'
                });

                // Total Items Value
                let totalItemsValueDiv: HTMLDivElement = totalItems.firstElementChild.children[1].firstElementChild as HTMLDivElement;
                CartViewUtils.setStyle(totalItemsValueDiv, {    
                    'fontFamily': 'Arial, sans-serif',
                    'fontSize': '16px',
                    'fontWeight': 'bold'
                });
            }

            if (totalItems) {
                // Lines Div
                let linesDiv: HTMLDivElement = totalItems.previousElementSibling as HTMLDivElement;
                if (linesDiv) {
                    CartViewUtils.setStyle(linesDiv, {
                        'backgroundColor': 'lightyellow',
                        'padding': '5px',
                        'color': 'black'
                    });

                    // Lines Text
                    let linesTextDiv: HTMLDivElement = linesDiv.firstElementChild.firstElementChild.firstElementChild as HTMLDivElement;
                    CartViewUtils.setStyle(linesTextDiv, {
                        'fontFamily': 'Arial, sans-serif',
                        'fontSize': '16px',
                        'fontWeight': 'bold'
                    });
                    // Lines Value
                    let linesValueDiv: HTMLDivElement = linesDiv.firstElementChild.children[1].firstElementChild as HTMLDivElement;
                    CartViewUtils.setStyle(linesValueDiv, {
                        'fontFamily': 'Arial, sans-serif',
                        'fontSize': '16px',
                        'fontWeight': 'bold'
                    });
                }
            }
        }
    }
}