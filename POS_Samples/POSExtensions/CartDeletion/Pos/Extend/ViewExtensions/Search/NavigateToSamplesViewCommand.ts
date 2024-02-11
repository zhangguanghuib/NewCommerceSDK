import { ClientEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";

import { StoreOperations } from "../../../DataService/DataServiceRequests.g";

export default class NavigateToSimpleExtensionViewCommand extends SearchView.ProductSearchExtensionCommandBase {
    /**
     * Creates a new instance of the NavigateToSimpleExtensionViewCommand class.
     * @param {IExtensionCommandContext<ProductDetailsView.IProductSearchToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<SearchView.IProductSearchToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "navigateToSimpleExtensionViewCommand";
        this.label = "Navigate to Samples View";
        this.extraClass = "iconGo";
    }

    /**
     * Initializes the command.
     * @param {ProductDetailsView.IProductDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: SearchView.IProductSearchExtensionCommandState): void {
        this.canExecute = true;
        this.isVisible = true;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        // this.context.navigator.navigate("SamplesView");
        let toDate: Date = new Date()
        let fromDate: Date = new Date(toDate.getDate() - 3);
        let searchCartsAsyncRequest: StoreOperations.SearchCartsAsyncRequest<StoreOperations.SearchCartsAsyncResponse>
            = new StoreOperations.SearchCartsAsyncRequest<StoreOperations.SearchCartsAsyncResponse>(fromDate, toDate);

        this.context.runtime.executeAsync(searchCartsAsyncRequest).then(
            (response: ClientEntities.ICancelableDataResult<StoreOperations.SearchCartsAsyncResponse>): Promise<string> => {
                let cartIds = response.data.result.map(c => c.Id).join('');
                return Promise.resolve(cartIds);
            }).then((cartId: string): void => {
                console.log(cartId);
            });
    }
}