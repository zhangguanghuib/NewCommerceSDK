
import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import CheckGiftCardBalanceResponse from "./CheckGiftCardBalanceResponse";

export default class CheckGiftCardBalanceRequest<TResponse extends CheckGiftCardBalanceResponse> extends ExtensionOperationRequestBase<TResponse> {
    constructor(correlationId: string) {
        super(5002 /* operationId */, correlationId);
    }
}