import SetReasonCodeToSelectedCartLineResponse from "./SetReasonCodeToSelectedCartLineResponse";
import SetReasonCodeToSelectedCartLineRequest from "./SetReasonCodeToSelectedCartLineRequest";

import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<SetReasonCodeToSelectedCartLineResponse> =
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<SetReasonCodeToSelectedCartLineRequest<SetReasonCodeToSelectedCartLineResponse>>> {

        let operationRequest: SetReasonCodeToSelectedCartLineRequest<SetReasonCodeToSelectedCartLineResponse> =
            new SetReasonCodeToSelectedCartLineRequest<SetReasonCodeToSelectedCartLineResponse>(correlationId);

        return Promise.resolve(<ClientEntities.ICancelableDataResult<SetReasonCodeToSelectedCartLineRequest<SetReasonCodeToSelectedCartLineResponse>>>{
            canceled: false,
            data: operationRequest
        });
    };

export default getOperationRequest;