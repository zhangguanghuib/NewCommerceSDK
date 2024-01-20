import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import RemoveAppliedCouponsOperationResponse from "./RemoveAppliedCouponsOperationResponse";

export default class RemoveAppliedCouponsOperationRequest<TResponse extends RemoveAppliedCouponsOperationResponse>
    extends ExtensionOperationRequestBase<TResponse> {
    constructor(correlationId: string) {
        super(6004, correlationId);
    }
}
