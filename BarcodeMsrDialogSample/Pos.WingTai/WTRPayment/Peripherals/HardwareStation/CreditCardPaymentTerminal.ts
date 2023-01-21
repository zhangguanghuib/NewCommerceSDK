/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { HardwareStationDeviceActionRequest, HardwareStationDeviceActionResponse } from "PosApi/Consume/Peripherals";

import { ClientEntities } from "PosApi/Entities";
import { IRuntime } from "PosApi/Framework/Runtime";
import { IPaymentTerminal } from "../IPaymentTerminal";
import { IWTR_PaymentTerminalEntity } from "./../IPaymentTerminal";
//import MessageDialog from "../../Controls/DialogSample/MessageDialog";
import { IWTR_PaymentTerminalRequest, IWTR_VoidPaymentTerminalRequest } from "HardwareStationContextExtensions";
/**
 * Interacts with the HW station fiscal register.
 */
export default class CreditCardPaymentTerminal implements IPaymentTerminal {
    private static CREDITCARD_PAYMENTTERMINAL_DEVICE_NAME: string = "CreditCardPaymentTerminal";
    //private static CREDITCARD_PAYMENTTERMINAL_MAKE_PAYMENT_REQUEST_NAME: string = "makeCreditCardPayment";
    private static CREDITCARD_PAYMENTTERMINAL_VOID_PAYMENT_REQUEST_NAME: string = "voidCardPayment";

    private extensionContextRuntime: IRuntime;

    /**
     * Initializes a new instance of the CreditCardPaymentTerminal class.
     * @param {IRuntime} extensionContextRuntime The extension context runtime to execute async requests.
     */
    constructor(extensionContextRuntime: IRuntime) {
        this.extensionContextRuntime = extensionContextRuntime;
    }

    public makePayment(comPort: string, amount: number, tenderType: number, callerContext?: any)
        : Promise<IWTR_PaymentTerminalEntity> {

        var paymentTerminalRequest: IWTR_PaymentTerminalRequest = this.getPaymentTerminalRequest(comPort, amount, tenderType, callerContext);
       

        //let MakeCreditCardPaymentRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
        //    new HardwareStationDeviceActionRequest(CreditCardPaymentTerminal.CREDITCARD_PAYMENTTERMINAL_DEVICE_NAME,
        //        CreditCardPaymentTerminal.CREDITCARD_PAYMENTTERMINAL_MAKE_PAYMENT_REQUEST_NAME, paymentTerminalRequest);

        let CREDITCARD_PAYMENTTERMINAL_DEVICE_NAME: string = "PaymentTerminals";
        //let FISCAL_REGISTER_IS_READY_REQUEST_NAME: string = "IsReady";
        let CREDITCARD_PAYMENTTERMINAL_MAKE_PAYMENT_REQUEST_NAME: string = "makeCreditCardPayment";

        let registerFiscalTransactionRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest(CREDITCARD_PAYMENTTERMINAL_DEVICE_NAME,
                CREDITCARD_PAYMENTTERMINAL_MAKE_PAYMENT_REQUEST_NAME, paymentTerminalRequest);

        return this.extensionContextRuntime.executeAsync(registerFiscalTransactionRequest)
            .then((result: ClientEntities.ICancelableDataResult<HardwareStationDeviceActionResponse>): any => {
                return result.data.response;
            });

        //return this.extensionContextRuntime.executeAsync(MakeCreditCardPaymentRequest)
        //    .then((result: ClientEntities.ICancelableDataResult<HardwareStationDeviceActionResponse>) => {
        //        if (result.data.response != null) {
        //            return result.data.response;
        //        }
        //    }).catch((reason) => {
        //        MessageDialog.show(callerContext, "Payment", true, true, false, "Error - " + callerContext.resources.getString(reason[0]._errorCode)).then(() => {
        //            callerContext.logger.logInformational("Credit Card Payment Terminal Error - " + reason);
        //        });
        //        return Promise.reject("Closed");
        //    });
    }

    public voidPayment(comPort: string, amount: number, tenderType: number, invoiceNumber: string, callerContext?: any)
        : Promise<IWTR_PaymentTerminalEntity> {

        var paymentTerminalRequest: IWTR_VoidPaymentTerminalRequest = null;

        paymentTerminalRequest = <IWTR_VoidPaymentTerminalRequest>{
            COMPort: comPort,
            Amount: amount,
            InvoiceNumber: invoiceNumber,
            DeviceName: CreditCardPaymentTerminal.CREDITCARD_PAYMENTTERMINAL_DEVICE_NAME,
            DeviceType: "WINDOWS",
        };

        let voidCreditCardPaymentRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest(CreditCardPaymentTerminal.CREDITCARD_PAYMENTTERMINAL_DEVICE_NAME,
                CreditCardPaymentTerminal.CREDITCARD_PAYMENTTERMINAL_VOID_PAYMENT_REQUEST_NAME, paymentTerminalRequest);

        return this.extensionContextRuntime.executeAsync(voidCreditCardPaymentRequest)
            .then((result: ClientEntities.ICancelableDataResult<HardwareStationDeviceActionResponse>) => {
                if (result.data.response != null) {
                    return result.data.response;
                }
            });
    }

    private getPaymentTerminalRequest(comPort: string, amount: number, tenderType: number, callerContext?: any): IWTR_PaymentTerminalRequest {

        var paymentTerminalRequest: IWTR_PaymentTerminalRequest = null;

        paymentTerminalRequest = <IWTR_PaymentTerminalRequest>{
            COMPort: comPort,
            Amount: amount,
            TenderType: tenderType,
            DeviceName: CreditCardPaymentTerminal.CREDITCARD_PAYMENTTERMINAL_DEVICE_NAME,
            DeviceType: "WINDOWS",
        };

        return paymentTerminalRequest;
    }
   

    
}
