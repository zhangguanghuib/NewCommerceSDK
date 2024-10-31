import * as CustomerDetailsView from "PosApi/Extend/Views/CustomerDetailsView";
import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class DisplayCustomerSummaryCommand extends CustomerDetailsView.CustomerDetailsExtensionCommandBase {

    private _customer: ProxyEntities.Customer;
    private _affiliations: ProxyEntities.CustomerAffiliation[];
    private _wishLists: ProxyEntities.CommerceList[];
    private _loyaltyCards: ProxyEntities.LoyaltyCard[];

    constructor(context: IExtensionCommandContext<CustomerDetailsView.ICustomerDetailsToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "emailCustomerCommand";
        this.label = "Send Email";
        this.extraClass = "iconLightningBolt";

        this._affiliations = [];
        this.affiliationAddedHandler = (data: CustomerDetailsView.CustomerDetailsAffiliationAddedData): void => {
            this._affiliations = data.affiliations;
        };

        this._wishLists = [];
        this.wishListsLoadedHandler = (data: CustomerDetailsView.CustomerDetailsWishListsLoadedData): void => {
            this._wishLists = data.wishLists;
        };

        this._loyaltyCards = [];
        this.loyaltyCardsLoadedHandler = (data: CustomerDetailsView.CustomerDetailsLoyaltyCardsLoadedData): void => {
            this._loyaltyCards = data.loyaltyCards;
        };
    }

    protected init(state: CustomerDetailsView.ICustomerDetailsExtensionCommandState): void {
        const customerSalesOrderListDiv: HTMLDivElement = document.getElementById('customerSalesOrderList') as HTMLDivElement;
        if (!ObjectExtensions.isNullOrUndefined(customerSalesOrderListDiv)) {
            const customerOrderHistory: HTMLDivElement = customerSalesOrderListDiv.parentElement.parentElement as HTMLDivElement;
            const mainContainer: HTMLDivElement = customerOrderHistory.parentElement as HTMLDivElement;
            mainContainer.insertBefore(customerOrderHistory, mainContainer.firstChild);
        }
    }

    protected execute(): void {
        let message: string = "Customer Account: " + this._customer.AccountNumber + " | ";
        message += "Affiliations: " + this._affiliations.map((a: ProxyEntities.CustomerAffiliation) => a.Name).join(", ") + " | ";
        message += "Loyalty Cards: " + this._loyaltyCards.map((lc: ProxyEntities.LoyaltyCard) => lc.CardNumber).join(", ") + " | ";
        message += "Wish Lists: " + this._wishLists.map((wl: ProxyEntities.CommerceList) => wl.Name).join(", ");
        console.log(message);

    }
}