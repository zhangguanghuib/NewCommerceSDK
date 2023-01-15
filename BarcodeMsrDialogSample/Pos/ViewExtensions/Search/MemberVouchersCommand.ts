/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";
import * as Views from "PosApi/Create/Views";
import { IMemberVouchersExtensionViewModelOptions } from "../../Views/MemberSignUpView/NavigationContracts";
export default class MemberVouchersCommand extends SearchView.CustomerSearchExtensionCommandBase {

    public _context: IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>;
    /**
     * Creates a new instance of the ViewCustomerSummaryCommand class.
     * @param {IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>) {
        super(context);
        this._context = context;
        this.id = "MemberVouchersCommand";
        //this.label = context.resources.getString("string_50006");
        this.label = "Member Vouchers";
        this.extraClass = Views.Icons.Money;
        this.canExecute = true;
        this.searchResultsSelectedHandler = (data: SearchView.CustomerSearchSearchResultSelectedData): void => {
        };

        this.searchResultSelectionClearedHandler = (): void => {
        };
    }

    /**
     * Initializes the command.
     * @param {SearchView.ICustomerDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: SearchView.ICustomerSearchExtensionCommandState): void {
        this.isVisible = true;
    }

    protected execute(): void {

        let options: IMemberVouchersExtensionViewModelOptions = {
            MemberNumber: "memberNo",
            CardNo: "cardNo",
            TransactionId: "transactionId"
        };
        this.context.navigator.navigate("MemberVouchersView", options);
    }
}