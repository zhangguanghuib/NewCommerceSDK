import ko from "knockout";
import * as Views from "PosApi/Create/Views";
import * as Controls from "PosApi/Consume/Controls";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";
import CartLinesViewModel from "./CartLinesViewModel";

export default class CartLinesView extends Views.CustomViewControllerBase {
    public cartLinesDataList: Controls.IDataList<ProxyEntities.CartLine>;
    public readonly viewModel: CartLinesViewModel;

    constructor(context: Views.ICustomViewControllerContext, state: Views.ICustomViewControllerBaseState) {
        super(context);
        this.viewModel = new CartLinesViewModel(context, this.state);
        this.state.title = "Cart lines";
    }

    /**
     * Bind the html element with view controller.
     *
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
        // Customized binding
        ko.applyBindings(this, element);

        // DataList
        let dataListOptions: Readonly<Controls.IDataListOptions<ProxyEntities.CartLine>> = {
            interactionMode: Controls.DataListInteractionMode.Invoke,
            //data: this.viewModel.currentCartLines,
            data: this.viewModel._cartLinesObservable,
            columns: [
                {
                    title: "ID",
                    ratio: 20,
                    collapseOrder: 2,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.ItemId) ? StringExtensions.EMPTY : cartLine.ItemId;
                    }
                },
                {
                    title: "Name", // Name
                    ratio: 50,
                    collapseOrder: 4,
                    minWidth: 100,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Description) ? StringExtensions.EMPTY : cartLine.Description;

                    }
                },
                {
                    title: "Quantity", // Quantity
                    ratio: 10,
                    collapseOrder: 3,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.Quantity) ? StringExtensions.EMPTY : cartLine.Quantity.toString();

                    }
                },
                {
                    title: "Discount", // Discount
                    ratio: 10,
                    collapseOrder: 1,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.DiscountAmount) ? StringExtensions.EMPTY : cartLine.DiscountAmount.toString();
                    }
                },
                {
                    title: "Cost", // Cost
                    ratio: 10,
                    collapseOrder: 5,
                    minWidth: 50,
                    computeValue: (cartLine: ProxyEntities.CartLine): string => {
                        return ObjectExtensions.isNullOrUndefined(cartLine.TotalAmount) ? StringExtensions.EMPTY : cartLine.TotalAmount.toString();
                    }
                }
            ]
        };

        let dataListRootElem: HTMLDivElement = element.querySelector("#cartLinesListView") as HTMLDivElement;
        this.cartLinesDataList = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "DataList", dataListOptions, dataListRootElem);

        this.viewModel.loadAsync().then(() => {
            this.cartLinesDataList.data = this.viewModel.currentCartLines;
        });
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}