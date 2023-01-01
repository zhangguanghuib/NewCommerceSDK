/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { CardPaymentEnquireGiftCardBalancePeripheralRequestHandler } from "PosApi/Extend/RequestHandlers/PeripheralsRequestHandlers";
import { CardPaymentEnquireGiftCardBalancePeripheralRequest, CardPaymentEnquireGiftCardBalancePeripheralResponse } from "PosApi/Consume/Peripherals";
import { ClientEntities } from "PosApi/Entities";

/**
 * Override request handler class for getting gift card.
 */
export default class CardPaymentCheckGCBalancePeripheralRequestHandlerExt extends CardPaymentEnquireGiftCardBalancePeripheralRequestHandler {

    /**
     * Executes the request handler asynchronously.
     * @param {CardPaymentEnquireGiftCardBalancePeripheralRequest<CardPaymentEnquireGiftCardBalancePeripheralResponse>} The request containing the response.
     * @return {Promise<ICancelableDataResult<CardPaymentEnquireGiftCardBalancePeripheralResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: CardPaymentEnquireGiftCardBalancePeripheralRequest<CardPaymentEnquireGiftCardBalancePeripheralResponse>):
        Promise<ClientEntities.ICancelableDataResult<CardPaymentEnquireGiftCardBalancePeripheralResponse>> {

        let extensionTransaction: ClientEntities.IExtensionTransaction = {
            ExtensionProperties: [
                {
                    Key: "TestPropertyKey",
                    Value: { StringValue: "TestPropertyValue" }
                }
            ]
        };

        let updatedRequest: CardPaymentEnquireGiftCardBalancePeripheralRequest<CardPaymentEnquireGiftCardBalancePeripheralResponse> =
            new CardPaymentEnquireGiftCardBalancePeripheralRequest<CardPaymentEnquireGiftCardBalancePeripheralResponse>(
                request.correlationId,
                request.paymentConnectorId,
                request.tenderInfo,
                extensionTransaction
            );

        return this.defaultExecuteAsync(updatedRequest);
    }
}