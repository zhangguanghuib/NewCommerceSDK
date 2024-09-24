import RefundTerminalTenderLineToCartResponse from "./RefundTerminalTenderLineToCartResponse";
import RefundTerminalTenderLineToCartRequest from "./RefundTerminalTenderLineToCartRequest";

import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<RefundTerminalTenderLineToCartResponse> =
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<RefundTerminalTenderLineToCartRequest<RefundTerminalTenderLineToCartResponse>>> {

        let tenderLine: ProxyEntities.TenderLine = null;
        let operationRequest: RefundTerminalTenderLineToCartRequest<RefundTerminalTenderLineToCartResponse> =
            new RefundTerminalTenderLineToCartRequest<RefundTerminalTenderLineToCartResponse>(correlationId, tenderLine);

        return Promise.resolve(<ClientEntities.ICancelableDataResult<RefundTerminalTenderLineToCartRequest<RefundTerminalTenderLineToCartResponse>>>{
            canceled: false,
            data: operationRequest
        });
    };

export default getOperationRequest;