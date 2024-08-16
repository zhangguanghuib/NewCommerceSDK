
import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import AVACViewPaymTenderOperationResponse from "./AVACViewPaymTenderOperationResponse";

/**
 * (Sample) Operation request for AVACViewPaymTendering a message to console.
 */
export default class AVACViewPaymTenderOperationRequest<TResponse extends AVACViewPaymTenderOperationResponse>
    extends ExtensionOperationRequestBase<TResponse> {
    constructor(correlationId: string) {
        super(5006, correlationId);
    }
}