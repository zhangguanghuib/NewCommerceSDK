import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as CustomerDetailsView from "PosApi/Extend/Views/CustomerDetailsView";

export default class CustomerCrossLoyaltyCommand extends CustomerDetailsView.CustomerDetailsExtensionCommandBase {
    /**
     * Creates a new instance of the CustomerCrossLoyaltyCommand class.
     * @param {IExtensionCommandContext<CustomerDetailsView.ICustomerDetailsToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<CustomerDetailsView.ICustomerDetailsToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "customerCrossLoyaltyCommand";
        this.label = "Cross Loyalty Discount";
        this.extraClass = "iconLightningBolt";
    }

    /**
     * Initializes the command.
     * @param {CustomerDetailsView.ICustomerDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: CustomerDetailsView.ICustomerDetailsExtensionCommandState): void {

        this.isVisible = false;
        this.canExecute = true;

        let loyaltyCardListDiv: HTMLDivElement = document.querySelector("#loyaltyCardList") as HTMLDivElement;
        loyaltyCardListDiv?.parentElement?.parentElement?.remove();

        let wishListDiv: HTMLDivElement = document.querySelector("#wishList") as HTMLDivElement;
        wishListDiv?.parentElement?.parentElement?.remove();

        let customerRecommendedProductsListDiv: HTMLDivElement = document.querySelector("#customerRecommendedProductsList") as HTMLDivElement;
        customerRecommendedProductsListDiv?.parentElement?.parentElement?.remove();
    }

    /**
     * Executes the command.
     */
    protected execute(): void {

    }
}