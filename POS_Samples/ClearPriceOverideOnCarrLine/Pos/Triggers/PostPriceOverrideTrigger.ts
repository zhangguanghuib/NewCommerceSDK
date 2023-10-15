
import { RefreshCartClientRequest, RefreshCartClientResponse } from "PosApi/Consume/Cart";
import { ClientEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";
import { StoreOperations } from "../DataService/DataServiceRequests.g";

export default class PostPriceOverrideTrigger extends Triggers.PostPriceOverrideTrigger {
    public execute(options: Triggers.IPostPriceOverrideTriggerOptions): Promise<void> {
        console.log("PostPriceOverrideTrigger " + JSON.stringify(options) + ".");

        let clearIsPriceOverrideOnCartRequest: StoreOperations.ClearIsPriceOverrideOnCartRequest<StoreOperations.ClearIsPriceOverrideOnCartResponse>
            = new StoreOperations.ClearIsPriceOverrideOnCartRequest(options.cart.Id);

        return this.context.runtime.executeAsync(clearIsPriceOverrideOnCartRequest)
            .then((): Promise<ClientEntities.ICancelableDataResult<RefreshCartClientResponse>> => {
                let request: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest<RefreshCartClientResponse>();
                return this.context.runtime.executeAsync(request);
            }).then(() => {
                Promise.resolve();
            });
    }

}