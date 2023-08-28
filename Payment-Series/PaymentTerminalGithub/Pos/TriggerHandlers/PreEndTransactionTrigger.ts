import * as Triggers from "PosApi/Extend/Triggers/TransactionTriggers";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import {
    GetCurrentCartClientRequest, GetCurrentCartClientResponse
} from "PosApi/Consume/Cart";

export default class PreEndTransactionTrigger extends Triggers.PreEndTransactionTrigger {

    public execute(options: Triggers.IPreEndTransactionTriggerOptions): Promise<ClientEntities.ICancelable> {
        console.log("Executing PreEndTransactionTrigger with options " + JSON.stringify(options) + ".");

        let currentCart: ProxyEntities.Cart;
        return this.context.runtime.executeAsync<GetCurrentCartClientResponse>(new GetCurrentCartClientRequest())
            .then((getCurrentCartClientResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>):
                Promise<ClientEntities.ICancelable> => {
                currentCart = getCurrentCartClientResponse.data.result;

                // If the current cart is not empty but without CustomerId
                if (!ObjectExtensions.isNullOrUndefined(currentCart) && ObjectExtensions.isNullOrUndefined(currentCart.CustomerId)) {
                    return Promise.reject(new ClientEntities.ExtensionError("Not allow to checkout without customer"));
                } else {
                    return Promise.resolve({ canceled: false, data: null });
                }
            });
    }
}