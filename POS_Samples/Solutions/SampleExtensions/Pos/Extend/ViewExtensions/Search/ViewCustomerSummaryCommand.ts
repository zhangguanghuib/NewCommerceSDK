import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";
import { ObjectExtensions } from "PosApi/TypeExtensions";
export default class ViewCustomerSummaryCommand extends SearchView.CustomerSearchExtensionCommandBase {

    /**
     * Creates a new instance of the ViewCustomerSummaryCommand class.
     * @param {IExtensionCommandContext<CustomerDetailsView.ICustomerSearchToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "viewCustomerSummaryCommand";
        this.label = "View Customer Summary";
        this.extraClass = "iconLightningBolt";
        this.isVisible = false;

        this.searchResultsSelectedHandler = (data: SearchView.CustomerSearchSearchResultSelectedData): void => {
        };

        this.searchResultSelectionClearedHandler = (): void => {
        };
    }

    /**
     * Initializes the command.
     * @param {CustomerDetailsView.ICustomerDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: SearchView.ICustomerSearchExtensionCommandState): void {
        this.isVisible = false;


        document.addEventListener('DOMContentLoaded', function () {
            let createNewCustAnchor = document.querySelector('[data-ax-bubble="searchView_displayZeroCustomersText_createCustomer"]') as HTMLAnchorElement;
            if (!ObjectExtensions.isNullOrUndefined(createNewCustAnchor)) {
                createNewCustAnchor.style.display = "none";
            }
        });

        //let timer = setInterval(() => {
        //    let createNewCustAnchor = document.querySelector('[data-ax-bubble="searchView_displayZeroCustomersText_createCustomer"]') as HTMLAnchorElement;
        //    if (!ObjectExtensions.isNullOrUndefined(createNewCustAnchor)) {
        //        createNewCustAnchor.style.display = "none";
        //        clearInterval(timer);
        //    }
        //}, 100);   
    }

    /**
     * Executes the command.
     */
    protected execute(): void {

    }
}