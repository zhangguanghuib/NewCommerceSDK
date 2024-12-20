import { ExtensionOperationRequestFactoryFunctionType } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { DateExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import SaveDataToSelectedCartLineRequest from "./SaveDataToSelectedCartLineRequest";
import SaveDataToSelectedCartLineResponse from "./SaveDataToSelectedCartLineResponse";
import SearchTransactionsDialog from "./../../Dialogs/SearchTransactionsDialog";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<SaveDataToSelectedCartLineResponse> =
    function (
        context: IExtensionContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<SaveDataToSelectedCartLineRequest<SaveDataToSelectedCartLineResponse>>> {

        let installationDate: string = DateExtensions.getDate().toDateString();

        let dialog: SearchTransactionsDialog = new SearchTransactionsDialog();
        return dialog.open().then((criteria: ProxyEntities.TransactionSearchCriteria) => {
            if (!ObjectExtensions.isNullOrUndefined(criteria)) {
                installationDate = criteria.StartDateTime.toDateString();
            }
        }).then(() => {
            let operationRequest: SaveDataToSelectedCartLineRequest<SaveDataToSelectedCartLineResponse> =
                new SaveDataToSelectedCartLineRequest(correlationId, installationDate);

            return Promise.resolve(<ClientEntities.ICancelableDataResult<SaveDataToSelectedCartLineRequest<SaveDataToSelectedCartLineResponse>>>{
                canceled: false,
                data: operationRequest
            });
        });
    };

export default getOperationRequest;