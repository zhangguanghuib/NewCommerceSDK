import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/OperationTriggers"
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, RefreshCartClientRequest, RefreshCartClientResponse } from "PosApi/Consume/Cart";
import * as Messages from "../DataService/DataServiceRequests.g";

export default class PreOperationTrigger extends Triggers.PreOperationTrigger {
    public execute(options: Triggers.IOperationTriggerOptions): Promise<Commerce.Client.Entities.ICancelable> {
        if (ObjectExtensions.isNullOrUndefined(options)) {
            // This will never happen, but is included to demonstrate how to return a rejected promise when validation fails.
            let error: ClientEntities.ExtensionError
                = new ClientEntities.ExtensionError("The options provided to the PreTenderPaymentTrigger were invalid. Please select a product and try again.");
            return Promise.reject(error);
        } 

        if (options.operationRequest.operationId == ProxyEntities.RetailOperation.PayCash
            || options.operationRequest.operationId == ProxyEntities.RetailOperation.PayCard) {

            let correlationId: string = this.context.logger.getNewCorrelationId();
            let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);

            return this.context.runtime.executeAsync(getCurrentCartClientRequest).then((response: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {
                return this.context.runtime.executeAsync(new Messages.StoreOperations.SimplePingGetRequest(response.data.result.Id));
            }).then(pingGetResponse => {
                let refreshCartClientRequest: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest();
                return this.context.runtime.executeAsync(refreshCartClientRequest);
            }).then(() => {
                return Promise.resolve({ canceled: false });
            });
        }

        return Promise.resolve({ canceled: false });    
    }
}