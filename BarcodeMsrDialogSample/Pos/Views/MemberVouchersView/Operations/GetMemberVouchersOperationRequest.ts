
import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import GetMemberVouchersOperationResponse from "./GetMemberVouchersOperationResponse";

/**
 * (Sample) Operation request for store hours.
 */
export default class GetMemberVouchersOperationRequest<TResponse extends GetMemberVouchersOperationResponse> extends ExtensionOperationRequestBase<TResponse> {
    constructor(correlationId: string) {
        super(5000, correlationId);
    }
}