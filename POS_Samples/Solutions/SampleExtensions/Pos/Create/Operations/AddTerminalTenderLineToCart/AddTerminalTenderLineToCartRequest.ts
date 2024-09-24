import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import { ProxyEntities } from "PosApi/Entities";

import AddTerminalTenderLineToCartResponse from "./AddTerminalTenderLineToCartResponse";

export default class AddTerminalTenderLineToCartRequest<TResponse extends AddTerminalTenderLineToCartResponse> extends ExtensionOperationRequestBase<TResponse> {
    public readonly tenderLine: ProxyEntities.TenderLine;

    constructor(correlationId: string, tenderLine: ProxyEntities.TenderLine) {
        super(6009, correlationId);
        this.tenderLine = tenderLine;
    }
}