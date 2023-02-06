import { GetReceiptEmailAddressClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { ClientEntities, ProxyEntities} from "PosApi/Entities";
import { StringExtensions } from "PosApi/TypeExtensions";
import {
    GetCurrentCartClientRequest, GetCurrentCartClientResponse
} from "PosApi/Consume/Cart";
import {
    GetCustomerClientRequest, GetCustomerClientResponse,
} from "PosApi/Consume/Customer";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class GetReceiptEmailAddressClientRequestHandlerExt extends GetReceiptEmailAddressClientRequestHandler {
    public executeAsync(request: Commerce.GetReceiptEmailAddressClientRequest<Commerce.GetReceiptEmailAddressClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<Commerce.GetReceiptEmailAddressClientResponse>> {

        let currentCart: ProxyEntities.Cart;
        return this.context.runtime.executeAsync<GetCurrentCartClientResponse>(new GetCurrentCartClientRequest())
            .then((getCurrentCartClientResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>):
                Promise<ClientEntities.ICancelableDataResult<GetCustomerClientResponse>> => {
                currentCart = getCurrentCartClientResponse.data.result;
                // Gets the current customer.
                let result: Promise<ClientEntities.ICancelableDataResult<GetCustomerClientResponse>>;
                if (!ObjectExtensions.isNullOrUndefined(currentCart) && !ObjectExtensions.isNullOrUndefined(currentCart.CustomerId)) {
                    let getCurrentCustomerClientRequest: GetCustomerClientRequest<GetCustomerClientResponse> =
                        new GetCustomerClientRequest(currentCart.CustomerId);
                    result = this.context.runtime.executeAsync<GetCustomerClientResponse>(getCurrentCustomerClientRequest);
                } else {
                    result = Promise.resolve({ canceled: false, data: new GetCustomerClientResponse(null) });
                }
                return result;
            })
            .then((getCurrentCustomerClientResponse: ClientEntities.ICancelableDataResult<GetCustomerClientResponse>):
                Promise<ProxyEntities.Customer> => {
                let currentCustomer: ProxyEntities.Customer = getCurrentCustomerClientResponse.data.result;
                return Promise.resolve(currentCustomer);
            }).then((currentCustomer: ProxyEntities.Customer) => {
                if (StringExtensions.isEmptyOrWhitespace(currentCustomer.Email) || StringExtensions.isEmptyOrWhitespace(currentCustomer.ReceiptEmail)) {
                    currentCustomer.ReceiptEmail = "guanghui03@microsoft.com";
                }
                return this.defaultExecuteAsync(request);
            });
    }

}
