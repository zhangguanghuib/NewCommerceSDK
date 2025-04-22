import * as PaymentView from "PosApi/Extend/Views/PaymentView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class PaymentViewCommand extends PaymentView.PaymentViewExtensionCommandBase {
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

        // Only allow button if it is for credit cards.
        if (state.tenderType.OperationId === Commerce.Proxy.Entities.RetailOperation.PayCard) {
            this.isVisible = true;
            this.canExecute = true;

            //this.paymentViewPaymentCardChangedHandler = (data: PaymentView.PaymentViewPaymentCardChanged): void => {

            this.context.logger.logInformational("Payment View Command - Payment card changed");
            let cardTypePanel: HTMLSelectElement = document.querySelector('#CardTypePanel') as HTMLSelectElement;
            if (!ObjectExtensions.isNullOrUndefined(cardTypePanel)) {
                cardTypePanel.style.visibility = "visible";
            }

            let billingAddressPanel: HTMLSelectElement = document.querySelector('#BillingAddressPanel') as HTMLSelectElement;
            if (!ObjectExtensions.isNullOrUndefined(billingAddressPanel)) {
                billingAddressPanel.style.visibility = "hidden";
            }

            };
        //} else {
        //    this.isVisible = false;
        //    this.canExecute = false;
        //}
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
    }
}