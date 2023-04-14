/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ClientEntities} from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import CreditCardPaymentTerminal from "./../Peripherals/HardwareStation/CreditCardPaymentTerminal";
import {IWTR_PaymentTerminalEntity} from "./../Peripherals/IPaymentTerminal";
/**
 * Implements fiscalization logic.
 */
export default class CreditCardTerminalManager {
    private static CREDITCARD_PAYMENTTERMINAL_NOT_AVAILABLE_ERROR_RESOURCE_ID: string = "CreditCard_PaymentTerminal_not_available_error";
    private extensionContext: IExtensionContext;

    /**
     * Initializes a new instance of the CreditCardTerminalManager class.
     * @param {IExtensionContext} extensionContext The extension context contained runtime to execute async requests.
     */
    constructor(extensionContext: IExtensionContext) {
        this.extensionContext = extensionContext;
    }

    /**
     * Checkes whether fiscal register is ready for fiscalization operation.
     * @return {Promise<void>} The result of fiscal register readyness check.
     */

    public makePayment(comPort: string, amount: number, tenderType: number, callerContext?: any)
        : Promise<IWTR_PaymentTerminalEntity> {
          
        let creditCardPaymentTerminal: CreditCardPaymentTerminal = new CreditCardPaymentTerminal(this.extensionContext.runtime);
        // Checks that HW station fiscal register is ready for registration
        return creditCardPaymentTerminal.makePayment(comPort, amount, tenderType, callerContext)
            .then((result): Promise<IWTR_PaymentTerminalEntity> => {
                return Promise.resolve(result);
            })
            .catch((reason: any) => {
                this.extensionContext.logger.logError("Credit Card make payment error: " + JSON.stringify(reason));

                return Promise.reject(new ClientEntities.ExtensionError(
                    this.extensionContext.resources.getString(CreditCardTerminalManager.CREDITCARD_PAYMENTTERMINAL_NOT_AVAILABLE_ERROR_RESOURCE_ID)));
            });
    }
}
