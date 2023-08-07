import RemoveAppliedCouponsOperationResponse from "./RemoveAppliedCouponsOperationResponse";
import RemoveAppliedCouponsOperationRequest from "./RemoveAppliedCouponsOperationRequest";
import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<RemoveAppliedCouponsOperationResponse> =

    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<RemoveAppliedCouponsOperationRequest<RemoveAppliedCouponsOperationResponse>>> {
        let operationRequest: RemoveAppliedCouponsOperationRequest<RemoveAppliedCouponsOperationResponse> =
            new RemoveAppliedCouponsOperationRequest<RemoveAppliedCouponsOperationResponse>(correlationId);

        return Promise.resolve(<ClientEntities.ICancelableDataResult<RemoveAppliedCouponsOperationRequest<RemoveAppliedCouponsOperationResponse>>>{
            canceled: false,
            data: operationRequest
        });
    };

export default getOperationRequest;