/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";
export default class NewMemberSignupCommand extends SearchView.CustomerSearchExtensionCommandBase {

    public _customerSearchResults: ProxyEntities.GlobalCustomer[];
    private _context: IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>;
    /**
     * Creates a new instance of the ViewCustomerSummaryCommand class.
     * @param {IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>) {
        super(context);
        this._context = context;
        this.id = "NewMemberSignupCommand";
        //this.label = context.resources.getString("string_50006");
        this.label = "New Memember Signup";
        this.extraClass = "iconAddFriend custom";
        this._customerSearchResults = [];
        this.canExecute = true;
        this.searchResultsSelectedHandler = (data: SearchView.CustomerSearchSearchResultSelectedData): void => {
            this._customerSearchResults = data.customers;
        };

        this.searchResultSelectionClearedHandler = (): void => {
            this._customerSearchResults = [];
           
        };
    }

    /**
     * Initializes the command.
     * @param {SearchView.ICustomerDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: SearchView.ICustomerSearchExtensionCommandState): void {
        this.isVisible = true;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        this._context.navigator.navigate("MemberSignUpView");
    }
}