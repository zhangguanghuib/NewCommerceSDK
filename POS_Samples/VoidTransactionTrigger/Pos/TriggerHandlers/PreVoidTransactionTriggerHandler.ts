import { RefreshCartClientRequest, RefreshCartClientResponse } from "PosApi/Consume/Cart";
import { ClientEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/TransactionTriggers";

import { StoreOperations } from "../DataService/DataServiceRequests.g";
export default class PreVoidTransactionTriggerHandler extends Triggers.PreVoidTransactionTrigger {
    public execute(options: Triggers.IPreVoidTransactionTriggerOptions): Promise<ClientEntities.ICancelable> {

        let request: StoreOperations.RemoveReceiptIdfromVoidedTransactionRequest<StoreOperations.RemoveReceiptIdfromVoidedTransactionResponse>
            = new StoreOperations.RemoveReceiptIdfromVoidedTransactionRequest(options.cart.Id);
        let corelationId: string = this.context.logger.getNewCorrelationId();

        return this.context.runtime.executeAsync(request)
            .then((result: ClientEntities.ICancelableDataResult<StoreOperations.RemoveReceiptIdfromVoidedTransactionResponse>)
                : Promise<ClientEntities.ICancelableDataResult<RefreshCartClientResponse>> => {
                if (!result.canceled) {

                    
                    let refreshCartClientRequest: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest(corelationId);
                    return this.context.runtime.executeAsync(refreshCartClientRequest);
                } else {
                    return Promise.resolve({ canceled: true, data: null });
                }
            }).then((result: ClientEntities.ICancelableDataResult<RefreshCartClientResponse>): Promise<ClientEntities.ICancelable> => {
                // this.context.navigator.navigateToPOSView("CartView");
                return Promise.resolve({ canceled: false, data: result.data.result });
            })
            .catch((err: any) => {
                this.context.logger.logError(JSON.stringify(err));
                console.log(err);
                return Promise.resolve({ canceled: true, data: null });
            });
    }
}