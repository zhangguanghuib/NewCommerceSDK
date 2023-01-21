/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as PaymentView from "PosApi/Extend/Views/PaymentView";
import MessageDialog from "../../Create/Dialogs/KREMessageDialog";
//import * as Messages from "../../DataService/DataServiceRequests.g";
//import { PaymentHandlerHelper } from "../../Handlers/PaymentHandlerHelper";
import { HardwareStationDeviceActionRequest, HardwareStationDeviceActionResponse } from "PosApi/Consume/Peripherals";

export default class KREPaymentComand extends PaymentView.PaymentViewExtensionCommandBase {

    public _paymentCard: Commerce.Proxy.Entities.PaymentCard;
    public _tenderType: Commerce.Proxy.Entities.TenderType;
    public _fullAmount: number;
    public _currency: Commerce.Proxy.Entities.Currency;
    public _paymentAmount: string;
    public _deviceName: string;
    public _deviceType: string;

    /**
     * Creates a new instance of the ButtonCommand class.
     * @param {IExtensionCommandContext<PaymentView.IPaymentToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<PaymentView.IPaymentViewToExtensionCommandMessageTypeMap>) {
        super(context);

        this.label = "Payment Connect dong";
        this.id = "paymentConnect";
        this.extraClass = "iconAccept";
    }

    /**
     * Initializes the command.
     * @param {PaymentView.IPaymentViewExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: PaymentView.IPaymentViewExtensionCommandState): void {
        //let edcClass = new Messages.KREPaymentEDCDataRequest();


        //edcClass.EdcName = "CIMB";
        //edcClass.EdcCom = "COM4";
        //edcClass.BaudRate = "115200";
        this._paymentCard = null;
        this._tenderType = state.tenderType;
        this._fullAmount = state.fullAmount;
        this._currency = state.currency;
        this._deviceName = "Ingenico Move2xxx (COM4)";
        this._deviceType = "Move2500";
        //this.isProcessing = true;

        // Only allow button if it is for credit cards
        //if (state.tenderType.OperationId === Commerce.Proxy.Entities.RetailOperation.PayCard) {
            this.isVisible = true;
            this.canExecute = true;

        //    this.paymentViewPaymentCardChangedHandler = (data: PaymentView.PaymentViewPaymentCardChanged): void => {
        //        this._paymentCard = data.paymentCard;
        //        this.context.logger.logInformational("Payment View Command - Payment card changed");
        //    };

        //    this.paymentViewAmountChangedHandler = (data: PaymentView.PaymentViewAmountChanged): void => {
        //        this._paymentAmount = data.paymentAmount;
        //        this.context.logger.logInformational("Payment View Command - Amount changed");
        //    };
        //} else {
        //    this.isVisible = false;
        //    this.canExecute = false;
        //}
    }

    //private getEdcName() {
    //    let edcClass = new Messages.KREPaymentEDCServices();
    //    let edcRequest = new Messages.KREPaymentEDCDataRequest();

    //      edcRequest = edcRequest.Where(f => f.GetEdcName().Equals(edcClass.getEdcNameCimb())).FirstOrDefault();
    //    if (edcRequest != null) {
    //            //this.btnConnectToEdc.Enabled = true;
    //            //this.btnConnectToEdc.Text += edc.GetEdcName();
    //            //logName = pathFileLog + edc.GetEdcName() + "_log.log";
    //        edcClass.EdcName = edcRequest.GetEdcName();
    //        edcClass.EdcCom = edcRequest.GetComName();
    //        edcClass.BaudRate = edcRequest.GetBaudRate();
    //            //qris = (cardInfo.CardTypeId.ToUpper().IndexOf("CIMB") >= 0 && cardInfo.CardTypeId.ToUpper() == "CIMB-RP") && tbCardNumber.Text.Trim().Length >= 1;
    //        }
    //}

    /**
     * Executes the command.
     */
    protected execute(): void {
        this.isProcessing = true;
        let hardwareStationDeviceActionRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest("KREPAYMENTEDC", "EdcControllerV2", { amount: "30", isQris: false });
        this.context.runtime.executeAsync(hardwareStationDeviceActionRequest).then
            ((result) => {
                MessageDialog.show(this.context, "PAYMENTEDC - EdcController ", "Message from HWS: " + result.data.response);
                this.isProcessing = false;
            }).catch((err) => {
                MessageDialog.show(this.context, "PAYMENTEDC - EdcController ", "Failure in executing HardwareStation request:" + err);
                this.isProcessing = false;
                throw err;
            });
    }
}
