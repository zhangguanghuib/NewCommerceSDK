import { GetReceiptEmailAddressClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { GetReceiptEmailAddressClientRequest, GetReceiptEmailAddressClientResponse } from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";

declare var Commerce: any;

export default class GetReceiptEmailAddressClientRequestHandlerExt extends GetReceiptEmailAddressClientRequestHandler {

    /**
     * Executes the request handler asynchronously.
     * @param {GetReceiptEmailAddressClientRequest<GetReceiptEmailAddressClientResponse>} The request containing the response.
     * @return {Promise<ICancelableDataResult<GetReceiptEmailAddressClientResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: GetReceiptEmailAddressClientRequest<GetReceiptEmailAddressClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>> {

        let customer: ProxyEntities.Customer;
        customer = Commerce.Session.instance.customerContext.customer;
        if (!ObjectExtensions.isNullOrUndefined(customer)) {
            customer.ReceiptEmail = "test1@test1.com";
        }

        return this.defaultExecuteAsync(request);
    }
}
