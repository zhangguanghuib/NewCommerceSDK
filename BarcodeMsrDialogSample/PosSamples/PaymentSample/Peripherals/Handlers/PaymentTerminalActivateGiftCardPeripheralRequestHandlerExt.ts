/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { PaymentTerminalActivateGiftCardPeripheralRequestHandler } from "PosApi/Extend/RequestHandlers/PeripheralsRequestHandlers";
import { PaymentTerminalActivateGiftCardPeripheralRequest, PaymentTerminalActivateGiftCardPeripheralResponse } from "PosApi/Consume/Peripherals";
import { ClientEntities } from "PosApi/Entities";

/**
 * Override request handler class for the payment terminal activate gift card.
 */
export default class PaymentTerminalActivateGiftCardPeripheralRequestHandlerExt extends PaymentTerminalActivateGiftCardPeripheralRequestHandler {

    /**
     * Executes the request handler asynchronously.
     * This sample calls the default behavior of activate gift card, replace with the customization to override the default behavior.
     * @param {PaymentTerminalActivateGiftCardPeripheralRequest<PaymentTerminalActivateGiftCardPeripheralResponse>} request The request.
     * @return {Promise<ICancelableDataResult<PaymentTerminalActivateGiftCardPeripheralResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: PaymentTerminalActivateGiftCardPeripheralRequest<PaymentTerminalActivateGiftCardPeripheralResponse>)
        : Promise<ClientEntities.ICancelableDataResult<PaymentTerminalActivateGiftCardPeripheralResponse>> {

        let extensionTransaction: ClientEntities.IExtensionTransaction = {
            ExtensionProperties: [
                {
                    Key: "TestPropertyKey",
                    Value: { StringValue: "TestPropertyValue" }
                }
            ]
        };

        let updatedRequest: PaymentTerminalActivateGiftCardPeripheralRequest<PaymentTerminalActivateGiftCardPeripheralResponse> =
            new PaymentTerminalActivateGiftCardPeripheralRequest<PaymentTerminalActivateGiftCardPeripheralResponse>(
                request.correlationId,
                request.amount,
                request.tenderInfo,
                request.paymentConnectorId,
                extensionTransaction
            );

        return this.defaultExecuteAsync(updatedRequest);
    }
}