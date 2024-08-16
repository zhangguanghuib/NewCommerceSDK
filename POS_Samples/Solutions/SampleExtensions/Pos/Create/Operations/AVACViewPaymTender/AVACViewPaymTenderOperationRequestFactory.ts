
import AVACViewPaymTenderOperationResponse from "./AVACViewPaymTenderOperationResponse";
import AVACViewPaymTenderOperationRequest from "./AVACViewPaymTenderOperationRequest";
import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<AVACViewPaymTenderOperationResponse> =
    /**
     * Gets an instance of AVACViewPaymTenderOperationRequest.
     * @param {number} operationId The operation Id.
     * @param {string[]} actionParameters The action parameters.
     * @param {string} correlationId A telemetry correlation ID, used to group events logged from this request together with the calling context.
     * @return {AVACViewPaymTenderOperationRequest<TResponse>} Instance of AVACViewPaymTenderOperationRequest.
     */
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<AVACViewPaymTenderOperationRequest<AVACViewPaymTenderOperationResponse>>> {

    let operationRequest: AVACViewPaymTenderOperationRequest<AVACViewPaymTenderOperationResponse> = new AVACViewPaymTenderOperationRequest<AVACViewPaymTenderOperationResponse>(correlationId);
    return Promise.resolve(<ClientEntities.ICancelableDataResult<AVACViewPaymTenderOperationRequest<AVACViewPaymTenderOperationResponse>>>{
        canceled: false,
        data: operationRequest
    });
};

export default getOperationRequest;