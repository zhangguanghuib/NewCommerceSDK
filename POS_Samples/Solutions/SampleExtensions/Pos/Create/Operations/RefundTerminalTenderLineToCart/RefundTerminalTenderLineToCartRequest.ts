import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import { ProxyEntities } from "PosApi/Entities";

import RefundTerminalTenderLineToCartResponse from "./RefundTerminalTenderLineToCartResponse";

export default class RefundTerminalTenderLineToCartRequest<TResponse extends RefundTerminalTenderLineToCartResponse> extends ExtensionOperationRequestBase<TResponse> {
    public readonly tenderLine: ProxyEntities.TenderLine;

    constructor(correlationId: string, tenderLine: ProxyEntities.TenderLine) {
        super(6010, correlationId);
        this.tenderLine = tenderLine;
    }
}