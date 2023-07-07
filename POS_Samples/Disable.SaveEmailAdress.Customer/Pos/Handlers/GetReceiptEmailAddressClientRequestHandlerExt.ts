import { GetReceiptEmailAddressClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { GetReceiptEmailAddressClientRequest, GetReceiptEmailAddressClientResponse } from "PosApi/Consume/Cart";
import { ClientEntities } from "PosApi/Entities";

export default class GetReceiptEmailAddressClientRequestHandlerExt extends GetReceiptEmailAddressClientRequestHandler {

    /**
     * Executes the request handler asynchronously.
     * @param {GetReceiptEmailAddressClientRequest<GetReceiptEmailAddressClientResponse>} The request containing the response.
     * @return {Promise<ICancelableDataResult<GetReceiptEmailAddressClientResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: GetReceiptEmailAddressClientRequest<GetReceiptEmailAddressClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetReceiptEmailAddressClientResponse>> {

        if (Object.prototype.hasOwnProperty.call(request, "disableSaveEmailToCustomer")) {
            request["disableSaveEmailToCustomer"] = true;
        } if (Object.prototype.hasOwnProperty.call(request, "customer")) {
            request["customer"] = null;
        }

        return this.defaultExecuteAsync(request);
    }
}