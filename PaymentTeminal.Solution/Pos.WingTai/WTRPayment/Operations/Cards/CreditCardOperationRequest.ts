

import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import PaymentOperationResponse from "../PaymentOperationResponse";


export default class CreditCardOperationRequest<TResponse extends PaymentOperationResponse> extends ExtensionOperationRequestBase<TResponse> {

    public static readonly OPERATION_ID: number = 10002;
    public static PAYMENT_METHOD: string = "";

    constructor(correlationId: string, paymentMethod: string) {
        CreditCardOperationRequest.PAYMENT_METHOD = paymentMethod;
        super(CreditCardOperationRequest.OPERATION_ID, correlationId);

    }
}