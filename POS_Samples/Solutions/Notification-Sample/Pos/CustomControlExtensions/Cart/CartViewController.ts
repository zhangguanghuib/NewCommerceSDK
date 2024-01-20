import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCartViewControllerContext } from "PosApi/Extend/Views/CartView";
import * as CartView from "PosApi/Extend/Views/CartView";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class CartViewController extends CartView.CartExtensionViewControllerBase {
    public _selectedCartLines: ProxyEntities.CartLine[];
    public _selectedTenderLines: ProxyEntities.TenderLine[];

    /**
    * Creates a new instance of the CartViewController class.
    * @param {IExtensionCartViewControllerContext} context The events Handler context.
    * @remarks The events handler context contains APIs through which a handler can communicate with POS.
    */
    constructor(context: IExtensionCartViewControllerContext) {
        super(context);
        this.cartLineSelectedHandler = (data: CartView.CartLineSelectedData): void => {
            this._selectedCartLines = data.cartLines;
        };

        this.cartLineSelectionClearedHandler = (): void => {
            this._selectedCartLines = undefined;
        };

        this.tenderLineSelectedHandler = (data: CartView.TenderLineSelectedData): void => {
            this._selectedTenderLines = data.tenderLines;
        };

        this.tenderLineSelectionClearedHandler = (): void => {
            this._selectedCartLines = undefined;
        };

        this.cartChangedHandler = (data: CartView.CartChangedData) => {

            let discountedNodes: HTMLCollectionOf<Element> = document.getElementsByClassName("tillLayout-CustomColumn2 textRight");

            if (!ObjectExtensions.isNullOrUndefined(discountedNodes) && discountedNodes.length > 0) {
                Array.from(discountedNodes).forEach(elem => {
                    let divElem: HTMLDivElement = elem as HTMLDivElement;
                    if (divElem.innerText === "DISCOUNTED_YES") {
                        divElem.parentElement.parentElement.style.backgroundColor = "red";
                    }
                });
            }
        };
    }
}