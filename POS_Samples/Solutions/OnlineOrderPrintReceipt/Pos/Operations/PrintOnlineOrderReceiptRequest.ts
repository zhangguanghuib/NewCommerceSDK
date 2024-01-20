import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import PrintOnlineOrderReceiptResponse from "./PrintOnlineOrderReceiptResponse";

export default class PrintOnlineOrderReceiptRequest<TResponse extends PrintOnlineOrderReceiptResponse>
    extends ExtensionOperationRequestBase<TResponse> {
    constructor(correlationId: string) {
        super(60004, correlationId);
    }
}