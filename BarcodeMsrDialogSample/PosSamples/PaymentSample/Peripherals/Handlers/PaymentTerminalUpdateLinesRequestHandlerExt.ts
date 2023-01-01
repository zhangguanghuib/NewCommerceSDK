/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { PaymentTerminalUpdateLinesRequestHandler } from "PosApi/Extend/RequestHandlers/PeripheralsRequestHandlers";
import { PaymentTerminalUpdateLinesRequest, PaymentTerminalUpdateLinesResponse } from "PosApi/Consume/Peripherals";
import { ClientEntities } from "PosApi/Entities";

/**
 * Override request handler class for the payment terminal update lines request.
 */
export default class PaymentTerminalUpdateLinesRequestHandlerExt extends PaymentTerminalUpdateLinesRequestHandler {

    /**
     * Executes the request handler asynchronously.
     * This sample calls the default behavior of payment update lines, replace with the customization to override the default behavior.
     * @param {PaymentTerminalUpdateLinesRequest<PaymentTerminalUpdateLinesResponse>} request The request.
     * @return {Promise<ICancelableDataResult<PaymentTerminalUpdateLinesResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: PaymentTerminalUpdateLinesRequest<PaymentTerminalUpdateLinesResponse>)
        : Promise<ClientEntities.ICancelableDataResult<PaymentTerminalUpdateLinesResponse>> {

        return this.defaultExecuteAsync(request);
    }
}