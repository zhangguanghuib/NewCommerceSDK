
import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import StoreHoursOperationResponse from "./StoreHoursOperationResponse";

/**
 * (Sample) Operation request for store hours.
 */
export default class StoreHoursOperationRequest<TResponse extends StoreHoursOperationResponse> extends ExtensionOperationRequestBase<TResponse> {
    constructor(correlationId: string) {
        super(3001, correlationId);
    }
}