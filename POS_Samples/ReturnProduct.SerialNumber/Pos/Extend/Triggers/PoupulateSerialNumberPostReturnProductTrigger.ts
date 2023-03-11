import * as Products from "PosApi/Consume/Products";
import { PostReturnProductTrigger } from "PosApi/Extend/Triggers/ProductTriggers";
import { ClientEntities} from "PosApi/Entities";

export default class PoupulateSerialNumberPostReturnProductTrigger extends PostReturnProductTrigger {
    public execute(options: Commerce.Triggers.IPostReturnProductTriggerOptions): Promise<void> {
        console.log("PostReturnProductTrigger");

        if (options.cartLinesForReturn.length <= 0) {
            return Promise.resolve();
        }

        let productIds: number[] = [options.cartLinesForReturn[0].ProductId];
        let getProductsByIdsClientRequest: Products.GetProductsByIdsClientRequest<Products.GetProductsByIdsClientResponse> =
            new Products.GetProductsByIdsClientRequest(productIds);

        this.context.runtime.executeAsync(getProductsByIdsClientRequest)
            .then((getProductsByIdsClientResponse: ClientEntities.ICancelableDataResult<Products.GetProductsByIdsClientResponse>) => {

            })

        let getSerialNumberClientRequest: Products.GetSerialNumberClientRequest = null;

        return Promise.resolve();
    }

}