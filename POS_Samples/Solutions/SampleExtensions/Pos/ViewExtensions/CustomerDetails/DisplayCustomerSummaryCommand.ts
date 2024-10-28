/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import * as CustomerDetailsView from "PosApi/Extend/Views/CustomerDetailsView";
import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ObjectExtensions } from "PosApi/TypeExtensions";


export default class DisplayCustomerSummaryCommand extends CustomerDetailsView.CustomerDetailsExtensionCommandBase {

    private _customer: ProxyEntities.Customer;
    private _affiliations: ProxyEntities.CustomerAffiliation[];
    private _wishLists: ProxyEntities.CommerceList[];
    private _loyaltyCards: ProxyEntities.LoyaltyCard[];

    /**
     * Creates a new instance of the EmailCustomerCommand class.
     * @param {IExtensionCommandContext<CustomerDetailsView.ICustomerDetailsToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
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

    /**
     * Initializes the command.
     * @param {CustomerDetailsView.ICustomerDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: CustomerDetailsView.ICustomerDetailsExtensionCommandState): void {
        //if (!state.isSelectionMode) {
        //    this.isVisible = true;
        //    this.canExecute = true;
        //    this._customer = state.customer;
        //    this._affiliations = state.customer.CustomerAffiliations || [];
        //}

        const customerSalesOrderListDiv: HTMLDivElement = document.getElementById('customerSalesOrderList') as HTMLDivElement;
        if (!ObjectExtensions.isNullOrUndefined(customerSalesOrderListDiv)) {
            const customerOrderHistory: HTMLDivElement = customerSalesOrderListDiv.parentElement.parentElement as HTMLDivElement;
            const mainContainer: HTMLDivElement = customerOrderHistory.parentElement as HTMLDivElement;
            mainContainer.insertBefore(customerOrderHistory, mainContainer.firstChild);
        }
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        let message: string = "Customer Account: " + this._customer.AccountNumber + " | ";
        message += "Affiliations: " + this._affiliations.map((a: ProxyEntities.CustomerAffiliation) => a.Name).join(", ") + " | ";
        message += "Loyalty Cards: " + this._loyaltyCards.map((lc: ProxyEntities.LoyaltyCard) => lc.CardNumber).join(", ") + " | ";
        message += "Wish Lists: " + this._wishLists.map((wl: ProxyEntities.CommerceList) => wl.Name).join(", ");
        console.log(message);

    }
}