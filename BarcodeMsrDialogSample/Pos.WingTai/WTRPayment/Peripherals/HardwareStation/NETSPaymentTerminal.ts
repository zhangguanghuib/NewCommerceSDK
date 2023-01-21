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
import { IWTR_PaymentTerminalRequest } from "HardwareStationContextExtensions";
/**
 * Interacts with the HW station fiscal register.
 */
export default class NETSPaymentTerminal implements IPaymentTerminal {
    private static NETS_PAYMENTTERMINAL_DEVICE_NAME: string = "PaymentTerminals";
    //private static NETS_PAYMENTTERMINAL_IS_READY_REQUEST_NAME: string = "IsReady";
    private static NETS_PAYMENTTERMINAL_MAKE_PAYMENT_REQUEST_NAME: string = "makeNETSPayment";

    private extensionContextRuntime: IRuntime;

    /**
     * Initializes a new instance of the NETSPaymentTerminal class.
     * @param {IRuntime} extensionContextRuntime The extension context runtime to execute async requests.
     */
    constructor(extensionContextRuntime: IRuntime) {
        this.extensionContextRuntime = extensionContextRuntime;
    }

    public makePayment(comPort: string, amount: number, tenderType: number, callerContext?: any)
        : Promise<IWTR_PaymentTerminalEntity> {

        var paymentTerminalRequest: IWTR_PaymentTerminalRequest = this.getPaymentTerminalRequest(comPort, amount, tenderType, callerContext);

        let MakeNETSPaymentRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest(NETSPaymentTerminal.NETS_PAYMENTTERMINAL_DEVICE_NAME,
                NETSPaymentTerminal.NETS_PAYMENTTERMINAL_MAKE_PAYMENT_REQUEST_NAME, paymentTerminalRequest);

        return this.extensionContextRuntime.executeAsync(MakeNETSPaymentRequest)
            .then((result: ClientEntities.ICancelableDataResult<HardwareStationDeviceActionResponse>): IWTR_PaymentTerminalEntity => {
                return result.data.response;
            });
    }

    private getPaymentTerminalRequest(comPort: string, amount: number, tenderType: number, callerContext?: any): IWTR_PaymentTerminalRequest {

        var paymentTerminalRequest: IWTR_PaymentTerminalRequest = null;

        paymentTerminalRequest = <IWTR_PaymentTerminalRequest>{
            COMPort: comPort,
            Amount: amount,
            TenderType: tenderType,
            DeviceName: NETSPaymentTerminal.NETS_PAYMENTTERMINAL_DEVICE_NAME,
            DeviceType: "WINDOWS",
        };

        return paymentTerminalRequest;
    }
    /**
     * Checks if HW station fiscal register is ready for registration operation.
     * @return {Promise<boolean>} True if fiscal printer is ready for registration, otherwise false.
     */
    //public isReady(): Promise<boolean> {
    //    let isReadyRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
    //        new HardwareStationDeviceActionRequest(NETSPaymentTerminal.NETS_PAYMENTTERMINAL_DEVICE_NAME, NETSPaymentTerminal.NETS_PAYMENTTERMINAL_IS_READY_REQUEST_NAME, true);

    //    return this.extensionContextRuntime.executeAsync(isReadyRequest)
    //        .then((result: ClientEntities.ICancelableDataResult<HardwareStationDeviceActionResponse>): boolean => {
    //            return result.data.response;
    //        });
    //}

    
}
