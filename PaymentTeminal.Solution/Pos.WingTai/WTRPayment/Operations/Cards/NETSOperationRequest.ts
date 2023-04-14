

import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import PaymentOperationResponse from "../PaymentOperationResponse";


export default class NETSOperationRequest<TResponse extends PaymentOperationResponse> extends ExtensionOperationRequestBase<TResponse> {

    public static readonly OPERATION_ID: number = 10003;
    public static PAYMENT_METHOD: string = "";

    constructor(correlationId: string, paymentMethod: string) {
        NETSOperationRequest.PAYMENT_METHOD = paymentMethod;
        super(NETSOperationRequest.OPERATION_ID, correlationId);

    }
}