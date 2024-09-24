import AddTerminalTenderLineToCartResponse from "./AddTerminalTenderLineToCartResponse";
import AddTerminalTenderLineToCartRequest from "./AddTerminalTenderLineToCartRequest";

import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<AddTerminalTenderLineToCartResponse> =
    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<AddTerminalTenderLineToCartRequest<AddTerminalTenderLineToCartResponse>>> {

        let tenderLine: ProxyEntities.TenderLine = null;
        let operationRequest: AddTerminalTenderLineToCartRequest<AddTerminalTenderLineToCartResponse> =
            new AddTerminalTenderLineToCartRequest<AddTerminalTenderLineToCartResponse>(correlationId, tenderLine);

        return Promise.resolve(<ClientEntities.ICancelableDataResult<AddTerminalTenderLineToCartRequest<AddTerminalTenderLineToCartResponse>>>{
            canceled: false,
            data: operationRequest
        });
    };

export default getOperationRequest;