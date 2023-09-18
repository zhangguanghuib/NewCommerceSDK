import ko from "knockout";
import { InventoryLookupOperationRequest, InventoryLookupOperationResponse } from "PosApi/Consume/OrgUnits";

import * as Views from "PosApi/Create/Views";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { IPostLogOnViewOptions } from "./NavigationContracts";

export default class PostLogOnView extends Views.CustomViewControllerBase {
    private _options: any;
    //public backgroundImageEncodingURL: ko.Computed<string>;
    public imageUrl: ko.Observable<string>;
    constructor(context: Views.ICustomViewControllerContext, options?: IPostLogOnViewOptions) {
        super(context);

        this.state.title = "PostLogOnView sample";
        this._options = options;

        //this.imageUrl = ko.observable("disney-031-620x414.jpg");

        this.imageUrl = ko.observable("https://2.bp.blogspot.com/-6FdNdwbpPa0/TmaBQ69_y0I/AAAAAAAAAQs/sQeN4HkYGSM/s1600/Disney+Castle.jpg");
        //this.backgroundImageEncodingURL = ko.computed(() => {
        //    return "url(disney-031-620x414.jpg)";
        //}, this);
    }

    /**
     * Bind the html element with view controller.
     *
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        var btn = document.getElementById("confirmBtn");
        btn.addEventListener('click', () => {
            //this._confirmLogin();
            const correlationId: string = this.context.logger.getNewCorrelationId();
            this.context.logger.logInformational("The view friend details button was clicked on the customer details friends panel.", correlationId);
            let customerDetailsOptions: ClientEntities.CustomerDetailsNavigationParameters
                = new ClientEntities.CustomerDetailsNavigationParameters("004005", correlationId);
            this.context.navigator.navigateToPOSView("CustomerDetailsView", customerDetailsOptions);
        });

        var carbtn = document.getElementById("cartBtn");
        carbtn.addEventListener('click', () => {
            this.context.navigator.navigateToPOSView("CartView");
        });

        var inventlookupBtn = document.getElementById("inventlookupBtn");
        inventlookupBtn.addEventListener('click', () => {
            const correlationId: string = this.context.logger.getNewCorrelationId();
            //let simpleProduct: ProxyEntities.SimpleProduct = new ProxyEntities.SimpleProductClass();
            //simpleProduct.ItemId = '0001';
            //let inventoryLookupOptions: ClientEntities.InventoryLookupNavigationParameters =
            //    new ClientEntities.InventoryLookupNavigationParameters(correlationId, simpleProduct)
            //this.context.navigator.navigateToPOSView("InventoryLookupView", inventoryLookupOptions);

            let inventoryLookupOperationRequest: InventoryLookupOperationRequest<InventoryLookupOperationResponse> =
                new InventoryLookupOperationRequest(22565421963, correlationId);
            this.context.runtime.executeAsync(inventoryLookupOperationRequest);      
        });

        let searchOrdersBtn = document.getElementById("searchOrdersBtn");
        searchOrdersBtn.addEventListener('click', () => {
            const correlationId: string = this.context.logger.getNewCorrelationId();
            let params: ClientEntities.SearchOrdersNavigationParameters =
                new ClientEntities.SearchOrdersNavigationParameters(correlationId);
            this.context.navigator.navigateToPOSView<"SearchOrdersView">("SearchOrdersView", params);
        });
    }

    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }

    /**
     * Confirm login.
     */
    public _confirmLogin(): void {
        this._options.resolveCallback();
    }

    public _viewFriendDetails(friend: ProxyEntities.Customer): void {
        const correlationId: string = this.context.logger.getNewCorrelationId();
        this.context.logger.logInformational("The view friend details button was clicked on the customer details friends panel.", correlationId);
        let customerDetailsOptions: ClientEntities.CustomerDetailsNavigationParameters
            = new ClientEntities.CustomerDetailsNavigationParameters(friend.AccountNumber, correlationId);

        this.context.navigator.navigateToPOSView("CustomerDetailsView", customerDetailsOptions);
    }
}
