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

        //return this.defaultExecuteAsync(request);
        let result: Promise<ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>> = this.defaultExecuteAsync(request);

        return result;
        //let result: Promise<ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>> = this.defaultExecuteAsync(request);

        //return result.then((ret: ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>)
        //    : Promise<ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>> => {
        //    if (ret.canceled) {
        //        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>>{ canceled: true, data: null });
        //    } else {
        //        ret.data.result.saveEmailOnCustomer = true;
        //        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>>{ canceled: false, data: ret.data });
        //    }
        //})
    }
}
