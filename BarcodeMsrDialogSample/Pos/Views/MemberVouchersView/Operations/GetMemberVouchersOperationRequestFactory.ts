import GetMemberVouchersOperationResponse from "./GetMemberVouchersOperationResponse";
import GetMemberVouchersOperationRequest from "./GetMemberVouchersOperationRequest";
import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<GetMemberVouchersOperationResponse> =
    /**
     * Gets an instance of GetMemberVouchersOperationRequest.
     * @param {number} operationId The operation Id.
     * @param {string[]} actionParameters The action parameters.
     * @param {string} correlationId A telemetry correlation ID, used to group events logged from this request together with the calling context.
     * @return {GetMemberVouchersOperationRequest<TResponse>} Instance of GetMemberVouchersOperationRequest.
     */
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<GetMemberVouchersOperationRequest<GetMemberVouchersOperationResponse>>> {
        let operationRequest: GetMemberVouchersOperationRequest<GetMemberVouchersOperationResponse> =
            new GetMemberVouchersOperationRequest<GetMemberVouchersOperationResponse>(correlationId);

        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetMemberVouchersOperationRequest<GetMemberVouchersOperationResponse>>>{
            canceled: false,
            data: operationRequest
        });
    };

export default getOperationRequest;