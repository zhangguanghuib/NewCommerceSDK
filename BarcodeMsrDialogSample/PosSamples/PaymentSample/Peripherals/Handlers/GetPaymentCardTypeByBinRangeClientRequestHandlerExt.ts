/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { GetPaymentCardTypeByBinRangeClientRequestHandler } from "PosApi/Extend/RequestHandlers/PaymentRequestHandlers";
import { GetPaymentCardTypeByBinRangeClientRequest, GetPaymentCardTypeByBinRangeClientResponse } from "PosApi/Consume/Payments";
import { ClientEntities } from "PosApi/Entities";

/**
 * Override request handler class for getting price request.
 */
export default class GetPaymentCardTypeByBinRangeClientRequestHandlerExt extends GetPaymentCardTypeByBinRangeClientRequestHandler {

    /**
     * Executes the request handler asynchronously.
     * @param {GetPaymentCardTypeByBinRangeClientRequest<GetPaymentCardTypeByBinRangeClientResponse>} The request containing the response.
     * @return {Promise<GetPaymentCardTypeByBinRangeClientRequest<GetPaymentCardTypeByBinRangeClientResponse>>}
     * The cancelable promise containing the response.
     */
    public executeAsync(request: GetPaymentCardTypeByBinRangeClientRequest<GetPaymentCardTypeByBinRangeClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetPaymentCardTypeByBinRangeClientResponse>> {

        return this.defaultExecuteAsync(request);
    }
}
