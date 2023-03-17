import * as ManageShiftsView from "PosApi/Extend/Views/ManageShiftsView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { Icons } from "PosApi/Create/Views";
import { ClientEntities } from "PosApi/Entities";

import { StoreOperations } from "../../../DataService/DataServiceRequests.g";

export default class ManageShiftsUpdCommand extends ManageShiftsView.ManageShiftsExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ManageShiftsView.IManageShiftsToExtensionCommandMessageTypeMap>) {
        super(context);
        this.label = "Delete Cart";
        this.id = "DelCart";
        this.extraClass = Icons.Delete;
    }

    protected init(state: ManageShiftsView.IManageShiftsExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }

    protected execute(): void {
        //console.log("delete cart");
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