import * as Products from "PosApi/Consume/Products";
import { PostReturnProductTrigger } from "PosApi/Extend/Triggers/ProductTriggers";
import {
    SaveExtensionPropertiesOnCartLinesClientResponse,
    SaveExtensionPropertiesOnCartLinesClientRequest,
    SetCartAttributesClientRequest,
    SetCartAttributesClientResponse,
    RefreshCartClientRequest,
    RefreshCartClientResponse
} from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";

import { StoreOperations } from "../../DataService/DataServiceRequests.g";

export default class PoupulateSerialNumberPostReturnProductTrigger extends PostReturnProductTrigger {
    public execute(options: Commerce.Triggers.IPostReturnProductTriggerOptions): Promise<void> {
        console.log("PostReturnProductTrigger");

        if (options.cartLinesForReturn.length <= 0) {
            return Promise.resolve();
        }

        let correlationId: string = this.context.logger.getNewCorrelationId();
        let currentCartLine: ProxyEntities.CartLine = options.cartLinesForReturn[0];
        let currentCart: ProxyEntities.Cart = options.cart;
        let productIds: number[] = [options.cartLinesForReturn[0].ProductId];
        let getProductsByIdsClientRequest: Products.GetProductsByIdsClientRequest<Products.GetProductsByIdsClientResponse> =
            new Products.GetProductsByIdsClientRequest(productIds);

        return this.context.runtime.executeAsync(getProductsByIdsClientRequest)
            .then((getProductsByIdsClientResponse: ClientEntities.ICancelableDataResult<Products.GetProductsByIdsClientResponse>): ProxyEntities.SimpleProduct => {
                //if (getProductsByIdsClientResponse.canceled || getProductsByIdsClientResponse.data.products.length <= 0) {
                //    return Promise.resolve(null);
                //} else {
                return getProductsByIdsClientResponse.data.products[0];
                //}
            }).then((simpleProduct: ProxyEntities.SimpleProduct): Promise<string> => {
                let getSerialNumberClientRequest: Products.GetSerialNumberClientRequest<Products.GetSerialNumberClientResponse> = new Products.GetSerialNumberClientRequest(simpleProduct, correlationId);
                return this.context.runtime.executeAsync(getSerialNumberClientRequest).then((result: ClientEntities.ICancelableDataResult<Products.GetSerialNumberClientResponse>): Promise<string> => {
                    let getSerialNumberClientResponse: Products.GetSerialNumberClientResponse = result.data;
                    return Promise.resolve(getSerialNumberClientResponse.result);
                });
            }).then((serailNum: string): Promise<ProxyEntities.Cart> => {
                currentCartLine.SerialNumber = serailNum;
                currentCart.CartLines.forEach((line: ProxyEntities.CartLine) => {
                    if (line.LineId === currentCartLine.LineId) {
                        line.SerialNumber = serailNum;
                    }
                });
                let updateCartLineReq: StoreOperations.UpdateCartLinesAsyncRequest<StoreOperations.UpdateCartLinesAsyncResponse> =
                    new StoreOperations.UpdateCartLinesAsyncRequest(currentCart.Id, currentCart.CartLines, "0", currentCart.Version);
                return this.context.runtime.executeAsync(updateCartLineReq)
                    .then((result: ClientEntities.ICancelableDataResult<StoreOperations.UpdateCartLinesAsyncResponse>): Promise<ProxyEntities.Cart> => {
                    return Promise.resolve(result.data.result);
                }).then((cart: ProxyEntities.Cart): Promise<ProxyEntities.Cart> => {
                    let refreshCartReq: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest(correlationId);
                    return this.context.runtime.executeAsync(refreshCartReq).then((result: ClientEntities.ICancelableDataResult<RefreshCartClientResponse>): Promise<ProxyEntities.Cart> => {
                        return Promise.resolve(result.data.result);
                    })
                }).then((cart: ProxyEntities.Cart): Promise<ProxyEntities.Cart> => {
                    let setCartAttributesClientRequest: SetCartAttributesClientRequest<SetCartAttributesClientResponse>
                        = new SetCartAttributesClientRequest(currentCart.AttributeValues, correlationId);
                    return this.context.runtime.executeAsync(setCartAttributesClientRequest)
                        .then((result: ClientEntities.ICancelableDataResult<SetCartAttributesClientResponse>): Promise<ProxyEntities.Cart> => {
                            return Promise.resolve(result.data.result);
                        });
                })
            }).then((cart: ProxyEntities.Cart): Promise<void> => {
                let extensionPropertiesOnCartLine: ClientEntities.IExtensionPropertiesOnCartLine = {
                    cartLineId: currentCartLine.LineId,
                    extensionProperties: currentCartLine.ExtensionProperties
                };
                let saveExtensionPropertiesOnCartLinesClientRequest: SaveExtensionPropertiesOnCartLinesClientRequest<SaveExtensionPropertiesOnCartLinesClientResponse> =
                    new SaveExtensionPropertiesOnCartLinesClientRequest([extensionPropertiesOnCartLine], correlationId);

                return this.context.runtime.executeAsync(saveExtensionPropertiesOnCartLinesClientRequest)
                    .then((response: ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>) => {
                        return Promise.resolve();
                    });
            }).catch((reason: any) => {
                console.log(reason);
            });
    }

}