import PrintOnlineOrderReceiptRequest from "./PrintOnlineOrderReceiptRequest";
import PrintOnlineOrderReceiptResponse from "./PrintOnlineOrderReceiptResponse";
import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<PrintOnlineOrderReceiptResponse> =
    /**
     * Gets an instance of StoreHoursOperationRequest.
     * @param {number} operationId The operation Id.
     * @param {string[]} actionParameters The action parameters.
     * @param {string} correlationId A telemetry correlation ID, used to group events logged from this request together with the calling context.
     * @return {StoreHoursOperationRequest<TResponse>} Instance of StoreHoursOperationRequest.
     */
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<PrintOnlineOrderReceiptRequest<PrintOnlineOrderReceiptResponse>>> {
        let operationRequest: PrintOnlineOrderReceiptRequest<PrintOnlineOrderReceiptResponse> =
            new PrintOnlineOrderReceiptRequest<PrintOnlineOrderReceiptResponse>(correlationId);

        return Promise.resolve(<ClientEntities.ICancelableDataResult<PrintOnlineOrderReceiptRequest<PrintOnlineOrderReceiptResponse>>>{
            canceled: false,
            data: operationRequest
        });
    };

export default getOperationRequest;