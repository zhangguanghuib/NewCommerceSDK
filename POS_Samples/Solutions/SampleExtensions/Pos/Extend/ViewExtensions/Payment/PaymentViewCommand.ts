/**
 * SAMPLE CODE NOTICE
 *
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import * as PaymentView from "PosApi/Extend/Views/PaymentView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { UpdatePaymentAmountData } from "PosApi/Extend/Views/PaymentView";
import { HardwareStationDeviceActionRequest, HardwareStationDeviceActionResponse } from "PosApi/Consume/Peripherals";

export default class PaymentViewCommand extends PaymentView.PaymentViewExtensionCommandBase {

    public _paymentCard: Commerce.Proxy.Entities.PaymentCard;
    public _tenderType: Commerce.Proxy.Entities.TenderType;
    public _fullAmount: number;
    public _currency: Commerce.Proxy.Entities.Currency;
    public _paymentAmount: string;

    /**
     * Creates a new instance of the ButtonCommand class.
     * @param {IExtensionCommandContext<PaymentView.IPaymentToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<PaymentView.IPaymentViewToExtensionCommandMessageTypeMap>) {
        super(context);

        this.label = "Payment Command";
        this.id = "PaymentCommand";
        this.extraClass = "iconLightningBolt";
    }

    /**
     * Initializes the command.
     * @param {PaymentView.IPaymentViewExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: PaymentView.IPaymentViewExtensionCommandState): void {
        this._paymentCard = null;
        this._tenderType = state.tenderType;
        this._fullAmount = state.fullAmount;
        this._currency = state.currency;

        // Only allow button if it is for credit cards.
        if (state.tenderType.OperationId === Commerce.Proxy.Entities.RetailOperation.PayCard) {
            this.isVisible = true;
            this.canExecute = true;

            this.paymentViewPaymentCardChangedHandler = (data: PaymentView.PaymentViewPaymentCardChanged): void => {
                this._paymentCard = data.paymentCard;
                this.context.logger.logInformational("Payment View Command - Payment card changed");
            };

            this.paymentViewAmountChangedHandler = (data: PaymentView.PaymentViewAmountChanged): void => {
                this._paymentAmount = data.paymentAmount;
                this.context.logger.logInformational("Payment View Command - Amount changed");
            };
        } else {
            this.isVisible = false;
            this.canExecute = false;
        }
    }

    /**
     * Executes the command.
     */
    protected execute(): void {

        let amount: number = 30;

        let hardwareStatationDeviceActionRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
            new HardwareStationDeviceActionRequest("COINDISPENSER",
                "DispenseChange", {
                Amount: amount,
                DeviceName: "MyCoinDispenser"
            });

        this.context.runtime.executeAsync(hardwareStatationDeviceActionRequest).then(() => {

            this.context.logger.logInformational("Hardware Station request executed successfully");
            let data: UpdatePaymentAmountData = {
                paymentAmount: 30
            };

            this.updatePaymentAmount(data);

        }).catch((err) => {
            this.context.logger.logInformational("Failure in executing Hardware Station request");
            throw err;
        });

      

    }
}