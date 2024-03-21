import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as CustomerDetailsView from "PosApi/Extend/Views/CustomerDetailsView";
//import { ArrayExtensions } from "PosApi/TypeExtensions";

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


        this.hidePhoneSections();

        //The below is for desktop Store Commerce
        // Find Loyalty Card Div
        let loyaltyCardListDiv: HTMLDivElement = document.querySelector("#loyaltyCardList") as HTMLDivElement;

        // Find loyalty Cars's Parent
        let sectionContainer: HTMLDivElement = loyaltyCardListDiv?.parentElement?.parentElement?.parentElement as HTMLDivElement;
        if (sectionContainer) {
            // Find times Line
            let timelineSection: HTMLDivElement = sectionContainer.children[1] as HTMLDivElement;
            timelineSection?.remove();
        }

        loyaltyCardListDiv?.parentElement?.parentElement?.remove();

        let wishListDiv: HTMLDivElement = document.querySelector("#wishList") as HTMLDivElement;
        wishListDiv?.parentElement?.parentElement?.remove();

        let customerRecommendedProductsListDiv: HTMLDivElement = document.querySelector("#customerRecommendedProductsList") as HTMLDivElement;
        customerRecommendedProductsListDiv?.parentElement?.parentElement?.remove();

        //let divcustomerDetailsViewPivot: HTMLDivElement = document.querySelector("#customerDetailsViewPivot");
        //let scrollContainer = divcustomerDetailsViewPivot.children[2];

        //let divViewWishList: HTMLDivElement = document.querySelector("#customerDetailsViewPivotWishList") as HTMLDivElement;
        //divViewWishList?.addEventListener("click", () => {
        //    divViewWishList?.parentElement?.remove();
        //});

        //console.dir(divcustomerDetailsViewPivot);
        //this.hideSections();
        //scrollContainer.addEventListener("scroll", () => {
        //    console.log("scrollContainer is scrolling");
        //    this.hideSections();
        //})


        //let nodeList: NodeListOf<HTMLButtonElement> = document.querySelectorAll("#customerDetailsViewPivot .win-pivot-header-area .win-pivot-header-items .win-pivot-header");
        //let btns: HTMLButtonElement[] = Array.from(nodeList);

        //btns.forEach(function (btn, index) {
        //    if (btn.innerText === 'Recommended products'
        //        || btn.innerText === 'Attributes'
        //        || btn.innerText === 'Loyalty cards'
        //        || btn.innerText === 'Wish lists'
        //        || btn.innerText === 'Affiliations'
        //        || btn.innerText === 'Attributes'
        //    ) {
        //        btn.style.display = "none";
        //    }
        //});

        //let divcustomerDetailsViewPivot: HTMLDivElement = document.querySelector("#customerDetailsViewPivot");
        //let scrollContainer = divcustomerDetailsViewPivot.children[2] as HTMLDivElement;
        //scrollContainer.style.visibility = "hidden";
        //scrollContainer.addEventListener("scroll", () => {
        //    let nodeList: NodeListOf<HTMLButtonElement> = document.querySelectorAll("#customerDetailsViewPivot .win-pivot-header-area .win-pivot-header-items .win-pivot-header");
        //    let btns: HTMLButtonElement[] = Array.from(nodeList);
        //    btns.forEach(function (btn, index) {
        //        if (btn.innerText === 'Recommended products'
        //            || btn.innerText === 'Attributes'
        //            || btn.innerText === 'Loyalty cards'
        //            || btn.innerText === 'Wish lists'
        //            || btn.innerText === 'Affiliations'
        //            || btn.innerText === 'Attributes'
        //        ) {
        //            btn.style.display = "none";
        //        }
        //    });
        //})
    }

    public hidePhoneSections(): void {
        let msg: string = `<div style="display: flex; justify-content: center; height: 100vh; align-items: center; text-align: center;">
               <div class="h3 wrapText">You don't have permission to view this page.</div>
            </div>`;

        let divViewActivities: HTMLDivElement = document.querySelector("#customerDetailsViewActivities") as HTMLDivElement;
        divViewActivities.innerHTML = msg;

        let divViewPivotLoyalty: HTMLDivElement = document.querySelector("#customerDetailsViewPivotLoyalty") as HTMLDivElement;
        divViewPivotLoyalty.innerHTML = msg;

        let divViewWishList: HTMLDivElement = document.querySelector("#customerDetailsViewPivotWishList") as HTMLDivElement;
        divViewWishList.innerHTML = msg;;

        let divViewViewPivotSuggestion: HTMLDivElement = document.querySelector("#customerDetailsViewPivotSuggestion") as HTMLDivElement;
        divViewViewPivotSuggestion.innerHTML = msg;

        let divViewPivotAffiliations: HTMLDivElement = document.querySelector("#customerDetailsViewPivotAffiliations") as HTMLDivElement;
        divViewPivotAffiliations.innerHTML = msg;

        let divViewPivotAdditionalProperties: HTMLDivElement = document.querySelector("#customerDetailsViewPivotAdditionalProperties") as HTMLDivElement;
        divViewPivotAdditionalProperties.innerHTML = msg;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {

    }
}