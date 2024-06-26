﻿import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";
import CheckGiftCardBalanceRequest from "./CheckGiftCardBalanceRequest";
import CheckGiftCardBalanceResponse from "./CheckGiftCardBalanceResponse";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<CheckGiftCardBalanceResponse> =
    /**
     * Gets an instance of CheckGiftCardBalanceRequest.
     * @param {number} operationId The operation Id.
     * @param {string[]} actionParameters The action parameters.
     * @param {string} correlationId A telemetry correlation ID, used to group events logged from this request together with the calling context.
     * @return {CheckGiftCardBalanceRequest<TResponse>} Instance of CheckGiftCardBalanceRequest.
     */
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<CheckGiftCardBalanceRequest<CheckGiftCardBalanceResponse>>> {

        let operationRequest: CheckGiftCardBalanceRequest<CheckGiftCardBalanceResponse> =
            new CheckGiftCardBalanceRequest<CheckGiftCardBalanceResponse>(correlationId);

        return Promise.resolve(<ClientEntities.ICancelableDataResult<CheckGiftCardBalanceRequest<CheckGiftCardBalanceResponse>>>{
            canceled: false,
            data: operationRequest
        });
    };

export default getOperationRequest;