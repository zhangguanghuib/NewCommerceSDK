import { Icons } from "PosApi/Create/Views";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";
//import { ProxyEntities/*, ClientEntities */} from "PosApi/Entities";

export default class ShowCustomerCommand extends SearchView.CustomerSearchExtensionCommandBase {

    //private _productSearchResultsSelectedData: ProxyEntities.ProductSearchResult[];

    constructor(context: IExtensionCommandContext<SearchView.ICustomerSearchToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "navigateCreateNewCustomerViewCommand";
        this.label = "Test create new customer";
        this.extraClass = Icons.Go;
    }

    protected init(state: SearchView.ICustomerSearchExtensionCommandState): void {
        this.canExecute = true;
        this.isVisible = true;
    }

    protected execute(): void {
        this.context.navigator.navigate("CnlCACustomerRegistrationView");
    }

}